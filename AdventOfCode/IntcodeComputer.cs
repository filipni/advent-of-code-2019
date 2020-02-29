using System;
using System.Collections.Generic;

namespace AdventOfCode
{
    class IntcodeComputer
    {
        private int pc;
        private int[] memory;
        private bool halted;

        public IntcodeComputer(int[] memory) => this.memory = memory;

        public void Run()
        {
            while (!halted)
            {
                int instructionLength = ExecuteInstruction();
                pc += instructionLength;
            }
        }

        private int ExecuteInstruction()
        {
            int instructionFormat = memory[pc];
            int opcode = GetDigits(instructionFormat, 0, 2);
            int paramsFormat = GetDigits(instructionFormat, 2, 3);
            List<int> paramPositions;

            switch (opcode)
            {
                case 1:
                    paramPositions = GetParamPositions(paramsFormat, 3);
                    return ADD(memory[paramPositions[0]], memory[paramPositions[1]], out memory[paramPositions[2]]);
                case 2:
                    paramPositions = GetParamPositions(paramsFormat, 3);
                    return MUL(memory[paramPositions[0]], memory[paramPositions[1]], out memory[paramPositions[2]]);
                case 3:
                    paramPositions = GetParamPositions(paramsFormat, 1);
                    return IN(out memory[paramPositions[0]]);
                case 4:
                    paramPositions = GetParamPositions(paramsFormat, 1);
                    return OUT(memory[paramPositions[0]]);
                case 5:
                    paramPositions = GetParamPositions(paramsFormat, 2);
                    return JT(memory[paramPositions[0]], memory[paramPositions[1]]);
                case 6:
                    paramPositions = GetParamPositions(paramsFormat, 2);
                    return JF(memory[paramPositions[0]], memory[paramPositions[1]]);
                case 7:
                    paramPositions = GetParamPositions(paramsFormat, 3);
                    return LET(memory[paramPositions[0]], memory[paramPositions[1]], out memory[paramPositions[2]]);
                case 8:
                    paramPositions = GetParamPositions(paramsFormat, 3);
                    return EQ(memory[paramPositions[0]], memory[paramPositions[1]], out memory[paramPositions[2]]);
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
                    index = pc + 1 + i;
                else
                    index = memory[pc + 1 + i];

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
            Console.Write("Input: "); 
            output = int.Parse(Console.ReadLine());
            return 2;
        }

        private int OUT(int a)
        {
            Console.WriteLine("Output: {0}", a);
            return 2;
        }

        private int JT(int a, int b)
        {
            if (a != 0)
            {
                pc = b;
                return 0;
            }
            return 3;
        }

        private int JF(int a, int b)
        {
            if (a == 0)
            {
                pc = b;
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
            halted = true;
            return 1;
        }
    }
}
