using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace AdventOfCode
{
    public class AmplifierSequence
    {
        private BlockingCollection<int> SequenceInput = new BlockingCollection<int>();
        private BlockingCollection<int> SequenceOutput;
        private List<IntcodeComputer> Amplifiers = new List<IntcodeComputer>();

        public AmplifierSequence(List<int> settingSequence, int[] program, bool feedbackOn = false)
        {
            BlockingCollection<int> inputQueue = SequenceInput;
            int lastIndex = settingSequence.Count - 1;

            for(int i = 0; i <= lastIndex; i++)
            {
                inputQueue.Add(settingSequence[i]);

                BlockingCollection<int> outputQueue;
                if (i == lastIndex && feedbackOn)
                    outputQueue = SequenceInput; 
                else
                    outputQueue = new BlockingCollection<int>();

                IntcodeComputer amp = new IntcodeComputer(program, inputQueue, outputQueue);
                Amplifiers.Add(amp);

                inputQueue = outputQueue;
            }

            SequenceOutput = inputQueue;
        }

        public int Run(int input)
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