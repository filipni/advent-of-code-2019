using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace AdventOfCode
{
    public class AmplifierSequence
    {
        private BlockingCollection<long> SequenceInput = new BlockingCollection<long>();
        private BlockingCollection<long> SequenceOutput;
        private List<IntcodeComputer> Amplifiers = new List<IntcodeComputer>();

        public AmplifierSequence(List<long> settingSequence, long[] program, bool feedbackOn = false)
        {
            BlockingCollection<long> inputQueue = SequenceInput;
            int lastIndex = settingSequence.Count - 1;

            for(int i = 0; i <= lastIndex; i++)
            {
                inputQueue.Add(settingSequence[i]);

                BlockingCollection<long> outputQueue;
                if (i == lastIndex && feedbackOn)
                    outputQueue = SequenceInput; 
                else
                    outputQueue = new BlockingCollection<long>();

                IntcodeComputer amp = new IntcodeComputer(program, inputQueue, outputQueue);
                Amplifiers.Add(amp);

                inputQueue = outputQueue;
            }

            SequenceOutput = inputQueue;
        }

        public long Run(int input)
        {
            SequenceInput.Add(input);

            // Start all amplifiers
            var tasks = new List<Task>();
            Amplifiers.ForEach(amp => tasks.Add(amp.Run()));

            Task.WaitAll(tasks.ToArray());

            return SequenceOutput.Last();
        }
    }
}