using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace AdventOfCode
{
    class IntcodeComputer
    {
        private long Pc;
        private long relativeBase;
        private const int MemorySize = 10000;
        private long[] Memory;
        private bool Halted;
        private BlockingCollection<long> InputQueue;
        private BlockingCollection<long> OutputQueue;

        public IntcodeComputer(long[] memory, BlockingCollection<long> inputQueue, BlockingCollection<long> outputqQueue)
        {
            Memory = Enumerable.Repeat(0L, MemorySize).ToArray();
            for (int i = 0; i < memory.Length; i++)
                Memory[i] = memory[i];

            InputQueue = inputQueue;
            OutputQueue = outputqQueue;
        }

        public Task Run() => Task.Run(_Run);

        private void _Run()
        {
            while (!Halted)
            {
                long instructionLength = ExecuteInstruction();
                Pc += instructionLength;
            }
        }

        private long ExecuteInstruction()
        {
            long instructionFormat = Memory[Pc];
            long opcode = GetDigits(instructionFormat, 0, 2);
            long paramsFormat = GetDigits(instructionFormat, 2, 3);
            List<long> paramPositions;

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
                case 9:
                    paramPositions = GetParamPositions(paramsFormat, 1);
                    return REL(Memory[paramPositions[0]]);
                case 99:
                    return HLT();
                default:
                    Console.WriteLine("Unknown opcode: {0}", opcode);
                    return 1;
            }
        }

        private List<long> GetParamPositions(long paramsFormat, int numParams)
        {
            var indices = new List<long>();

            for (int i = 0; i < numParams; i++)
            {
                long parameterMode = GetDigits(paramsFormat, i, 1);
                long index;

                switch (parameterMode)
                {
                    case 1: // Immediate mode
                        index = Pc + 1 + i;
                        break;
                    case 2: // Relative mode
                        index = relativeBase + Memory[Pc + 1 + i];
                        break;
                    default: // Position mode
                        index = Memory[Pc + 1 + i];
                        break;
                }

                indices.Add(index);
            }

            return indices;
        }

        private long GetDigits(long num, int start, int length)
        {
            start = (int)Math.Pow(10, start);
            length = (int)Math.Pow(10, length);
            return num / start % length;
        }

        private long ADD(long a, long b, out long output)
        {
            output = a + b;
            return 4;
        }

        private long MUL(long a, long b, out long output)
        {
            output = a * b;
            return 4;
        }

        private long IN(out long output)
        {
            output = InputQueue.Take();
            return 2;
        }

        private long OUT(long a)
        {
            OutputQueue.Add(a);
            return 2;
        }

        private long JT(long a, long b)
        {
            if (a != 0)
            {
                Pc = b;
                return 0;
            }
            return 3;
        }

        private long JF(long a, long b)
        {
            if (a == 0)
            {
                Pc = b;
                return 0;
            }
            return 3;
        }

        private int LET(long a, long b, out long output)
        {
            if (a < b)
                output = 1;
            else 
                output = 0;
            return 4;
        }

        private long EQ(long a, long b, out long output)
        {
            if (a == b)
                output = 1;
            else
                output = 0;
            return 4;
        }

        private long HLT()
        {
            Halted = true;
            OutputQueue.CompleteAdding();
            return 1;
        }

        private long REL(long a)
        {
            relativeBase += a;
            return 2;
        }
    }
}
