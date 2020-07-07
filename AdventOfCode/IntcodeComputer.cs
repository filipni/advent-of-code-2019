using System;
using System.Collections.Generic;

namespace AdventOfCode
{
    class IntcodeComputer
    {
        private int Pc;
        private int[] Memory;
        private bool Halted;
        private bool ExitAtOutput;
        private Queue<int> Input = new Queue<int>();
        private List<int> Output = new List<int>();

        public IntcodeComputer(int[] memory, IEnumerable<int> input = null, bool exitAtOutput = false)
        {
            this.Memory = memory;
            if (input != null)
                Input = new Queue<int>(input);
            ExitAtOutput = exitAtOutput;
        }

        public List<int> Run()
        {
            while (!Halted)
            {
                int instructionLength = ExecuteInstruction();
                Pc += instructionLength;
            }
            return new List<int>(Output);
        }

        private int ExecuteInstruction()
        {
            int instructionFormat = Memory[Pc];
            int opcode = GetDigits(instructionFormat, 0, 2);
            int paramsFormat = GetDigits(instructionFormat, 2, 3);
            List<int> paramPositions;

            switch (opcode)
            {
                case 1:
                    paramPositions = GetParamPositions(paramsFormat, 3);
                    return ADD(Memory[paramPositions[0]], Memory[paramPositions[1]], out Memory[paramPositions[2]]);
                case 2:
                    paramPositions = GetParamPositions(paramsFormat, 3);
                    return MUL(Memory[paramPositions[0]], Memory[paramPositions[1]], out Memory[paramPositions[2]]);
                case 3:
                    paramPositions = GetParamPositions(paramsFormat, 1);
                    return IN(out Memory[paramPositions[0]]);
                case 4:
                    paramPositions = GetParamPositions(paramsFormat, 1);
                    return OUT(Memory[paramPositions[0]]);
                case 5:
                    paramPositions = GetParamPositions(paramsFormat, 2);
                    return JT(Memory[paramPositions[0]], Memory[paramPositions[1]]);
                case 6:
                    paramPositions = GetParamPositions(paramsFormat, 2);
                    return JF(Memory[paramPositions[0]], Memory[paramPositions[1]]);
                case 7:
                    paramPositions = GetParamPositions(paramsFormat, 3);
                    return LET(Memory[paramPositions[0]], Memory[paramPositions[1]], out Memory[paramPositions[2]]);
                case 8:
                    paramPositions = GetParamPositions(paramsFormat, 3);
                    return EQ(Memory[paramPositions[0]], Memory[paramPositions[1]], out Memory[paramPositions[2]]);
                case 99:
                    return HLT();
                default:
                    Console.WriteLine("Unknown opcode: {0}", opcode);
                    return 1;
            }
        }

        private List<int> GetParamPositions(int paramsFormat, int numParams)
        {
            var indices = new List<int>();

            for (int i = 0; i < numParams; i++)
            {
                bool immediateMode = GetDigits(paramsFormat, i, 1) == 1;
                int index;

                if (immediateMode)
                    index = Pc + 1 + i;
                else
                    index = Memory[Pc + 1 + i];

                indices.Add(index);
            }

            return indices;
        }

        private int GetDigits(int num, int start, int length)
        {
            start = (int)Math.Pow(10, start);
            length = (int)Math.Pow(10, length);
            return num / start % length;
        }

        private int ADD(int a, int b, out int output)
        {
            output = a + b;
            return 4;
        }

        private int MUL(int a, int b, out int output)
        {
            output = a * b;
            return 4;
        }

        private int IN(out int output)
        {
            output = Input.Dequeue();
            return 2;
        }

        private int OUT(int a)
        {
            Output.Add(a);
            Halted = ExitAtOutput;
            return 2;
        }

        private int JT(int a, int b)
        {
            if (a != 0)
            {
                Pc = b;
                return 0;
            }
            return 3;
        }

        private int JF(int a, int b)
        {
            if (a == 0)
            {
                Pc = b;
                return 0;
            }
            return 3;
        }

        private int LET(int a, int b, out int output)
        {
            if (a < b)
                output = 1;
            else 
                output = 0;
            return 4;
        }

        private int EQ(int a, int b, out int output)
        {
            if (a == b)
                output = 1;
            else
                output = 0;
            return 4;
        }

        private int HLT()
        {
            Halted = true;
            return 1;
        }
    }
}
