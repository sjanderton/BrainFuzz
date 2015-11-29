using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BrainFuzzInterpreter
{
    public class BFInt
    {
        public enum EofSetting { EOF0, EOFNEG1, EOFNOCHANGE };

        private List<byte> tape;
        private string input;
        private string prog;
        private int pc = 0;
        private int ic = 0;
        private int p;
        private string output = "";
        private EofSetting setting;
        private Dictionary<int, int> sqOpenBrace;
        private Dictionary<int, int> sqCloseBrace;
        private Dictionary<int, int> rndOpenBrace;

        private byte register = 0;

        private bool inverNext = false;

        private bool supportExtentions;

        public BFInt(string program, string input, EofSetting set, bool useExtentions)
        {
            //initialize variables
            sqOpenBrace = new Dictionary<int, int>();
            sqCloseBrace = new Dictionary<int, int>();
            rndOpenBrace = new Dictionary<int, int>();
            tape = new List<byte>();

            supportExtentions = useExtentions;

            if (program == null)
            {
                throw (new Exception("Program cannot be null."));
            }
            else
            {
                //preprocess the program
                prog = processProgram(program);
            }
            this.input = input;
            
            //make our initial cell
            tape.Add(0);

            setting = set;
        }
        /*
         * processes 1 character
         * returns false if theres no more characters to process
         *
         * */
        public bool next()
        {
            if (pc >= prog.Length) { return false; } //check theres a character to process
            char op = prog[pc++]; //get current command and increase pc
            switch (op)
            {
                case '+':
                    tape[p]++;
                    break;
                case '-':
                    tape[p]--;
                    break;
                case '>':
                    if (++p >= tape.Count) //increase pointer and check if its outside tape
                    {
                        tape.Add(0); //make tape longer
                    }
                    break;
                case '<':
                    if (--p < 0)//increase pointer and check if its outside tape
                    {
                        tape.Insert(0, 0); //make tape longer
                        p = 0;
                    }
                    break;
                case '.':
                    output += (char)tape[p];
                    break;
                case ',':
                    if (ic < input.Length)//if theres something to read
                    {
                        tape[p] = (byte)input[ic++]; //read it
                    }
                    else//return EOF based on preference set in constructor
                    {
                        switch (setting)
                        {
                            case EofSetting.EOF0:
                                tape[p] = 0;
                                break;
                            case EofSetting.EOFNEG1:
                                tape[p] = 255; //bytes are unsigned so -1 = 255
                                break;
                            case EofSetting.EOFNOCHANGE:
                                break;
                            default:
                                break;
                        }

                    }
                    break;
                case ':':
                    output += tape[p]; //just add the number to output string
                    break;
                case ';':
                    //TODO test ascii to num
                    //reads in number and one more character
                    if (ic < input.Length)
                    {
                        int t = ic;
                        while (t < input.Length && input[t] <= '9' && input[t] >= '0')
                        {
                            t++;
                        }
                        if (t != ic)
                        {
                            string s = input.Substring(ic, t-ic);
                            ic += t-ic+1;
                            int x = Int32.Parse(s);
                            tape[p] = (byte)x;
                        }
                    }
                    else
                    {//if theres no number here return EOF
                        switch (setting)
                        {
                            case EofSetting.EOF0:
                                tape[p] = 0;
                                break;
                            case EofSetting.EOFNEG1:
                                tape[p] = 255;
                                break;
                            case EofSetting.EOFNOCHANGE:
                                break;
                            default:
                                break;
                        }

                    }
                    break;
                case '(':
                    if (tape[p] == 0 ^ inverNext)
                    {
                        int newpc = 0;
                        int ind = pc - 1;
                        if (rndOpenBrace.TryGetValue(ind, out newpc))
                        {
                            pc = newpc + 1;
                        }
                        else
                        {
                            throw (new Exception("Error while jumping forward to bracket!"));
                        }
                    }
                    inverNext = false;
                    break;
                case ')':
                    //dont jump back, just go to next instruction
                    //its only an if!
                    break;
                case '[':
                    if (tape[p] == 0 ^ inverNext)
                    {
                        int newpc = 0;
                        int ind = pc - 1;
                        if (inverNext)//if inverNext is true the matching bracket is located at index of '[' - 1 so the ! can be executed again.
                            ind--;
                        if (sqOpenBrace.TryGetValue(ind, out newpc))
                        {
                            pc = newpc + 1;
                        }
                        else
                        {
                            throw (new Exception("Error while jumping forward to bracket!"));
                        }
                        
                    }
                    inverNext = false;
                    break;
                case ']':
                    {
                        int newpc = 0;
                        if (sqCloseBrace.TryGetValue(pc - 1, out newpc))
                        {
                            pc = newpc;
                        }
                        else
                        {
                            throw (new Exception("Error while jumping back to bracket!"));
                        }
                    }
                    break;
                case '!':
                    if(pc < prog.Length)
                        if (prog[pc] == '(' || prog[pc] == '[' || prog[pc] == '"')
                        {
                            inverNext = true;
                        }
                    break;
                case '"'://Print string operator OR if !" read string
                    string word = "";
                    while (pc < prog.Length && prog[pc] != '"')
                    {
                        word += prog[pc++];
                    }
                    word = Regex.Unescape(word);
                    if (!inverNext)
                    {
                        output += word;
                    }
                    else
                    {
                        for (int i = 0; i < word.Length; i++)
                        {
                            tape[p++] = (byte)word[i];
                            if (p >= tape.Count)
                                tape.Add(0);
                        }
                    }
                    inverNext = false;
                    pc++;
                    break;
                case '^':
                    register = tape[p];
                    break;
                case '=':
                    tape[p] = register;
                    break;
                default:
                    break;
            }
            return true;
        }
        /*
         * return the output string and set it to 0 after
         * acts as a buffer
         * */
        public string getOutput()
        {
            string ret = output;
            output = "";
            return ret;
        }

        private string processProgram(string program)
        {
            string chars = "+-<>,.[]"; //standard BF chars

            if (supportExtentions)
            {
                chars += "()!;:\"^="; //add extended chars
            }

            string ret = "";
            //remove anything that isn't an BF operation and anything after // until newline
            for (int i = 0; i < program.Length; i++)
            {
                char c = program[i];
                if (chars.Contains(c.ToString()))
                {
                    ret += c;
                    if (c == '"')
                    {
                        string word = "";
                        i++;
                        while (i < program.Length && program[i] != '"')
                        {
                            if (!char.IsControl(program[i]))
                            {
                                word += program[i];
                            }
                            i++;
                        }
                        word = word.Replace(@"\n", @"\r\n");//make newlines windows compatible....
                        ret += word + "\"";
                    }
                }
                else if (c == '/')//remove anything after '//' until newline
                {
                    if (i + 1 < program.Length)
                    {
                        if (program[i + 1] == '/')
                        {
                            while (i < program.Length && program[i] != '\n') { i++; }
                        }
                    }
                }
                
            }

            //now generate a map of the brackets
            Stack<int> sqStack = new Stack<int>();
            Stack<int> rndStack = new Stack<int>();
            for (int i = 0; i < ret.Length; i++)
            {
                if (ret[i] == '[')
                {
                    if (i - 1 >= 0 && ret[i - 1] == '!')
                    {
                        sqStack.Push(i-1);
                    }
                    else
                    {
                        sqStack.Push(i);
                    }
                    
                }
                else if (ret[i] == ']')
                {
                    if (sqStack.Count != 0)
                    {
                        int v = sqStack.Pop();
                        sqOpenBrace.Add(v, i);
                        sqCloseBrace.Add(i, v);
                    }
                    else
                    {
                        throw (new Exception("Brackets don't match!"));
                    }

                }
                if (ret[i] == '(')
                {
                        rndStack.Push(i);
                }
                else if (ret[i] == ')')
                {
                    if (rndStack.Count != 0)
                    {
                        int v = rndStack.Pop();
                        rndOpenBrace.Add(v, i);
                    }
                    else
                    {
                        throw (new Exception("Brackets don't match!"));
                    }
                }

            }
            if (sqStack.Count != 0 || rndStack.Count != 0)
                throw (new Exception("Brackets don't match!"));
            return ret;
        }
        public string getProg()
        {
            return prog;
        }
    }
}
