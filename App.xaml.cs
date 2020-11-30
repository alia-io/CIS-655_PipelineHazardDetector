using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PipelineHazardDetector {

    enum InstructionType { add, sub, load, store }
    enum PipelineStage { STALL, IF, ID, EX, MEM, WB }
    //enum Registers { zero, at, v0, v1, a0, a1, a2, a3, t0, t1, t2, t3, t4, t5, t6, t7, s0, s1, s2, s3, s4, s5, s6, s7, t8, t9, k0, k1, gp, fp, ra }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        // pipelineType: 1 = with hazards, 2 = without forwarding, 3 = with forwarding
        public static void ParseInstructions(string input, int pipelineType) {

            // Separate each instruction
            String[] instructionArray = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int instructionNumber = 0; // counts the number of instructions
            //Console.WriteLine(String.Join(",", instructions));

            Queue<String> instructions = new Queue<String>(instructionArray); // put instructions into FIFO queue
            Queue<Instruction> currentInstructions = new Queue<Instruction>(); // used to hold instructions currently in execution

            // add the first instruction to the current instructions queue
            currentInstructions.Enqueue(new Instruction(instructions.Dequeue(), ++instructionNumber));

            /*
            Console.WriteLine("Original Queue:");
            foreach (String instruction in instructions) {
                Console.WriteLine(instruction);
            }
            instructions.Dequeue();
            Console.WriteLine("Queue with first value removed:");
            foreach (String instruction in instructions) {
                Console.WriteLine(instruction);
            }
            */

            //Pipeline pipeline = new Pipeline(instructions, pipelineType);
        }
    }

    public class Instruction {

        int number;               // which instruction it is, in written order
        InstructionType type;       // type of instruction: add, sub, load, or store
        DataLocation source1;       // source operand 1
        DataLocation source2;       // source operand 2 (if applicable)
        DataLocation destination;   // register or memory location to write to
        PipelineStage stage;        // last completed pipeline stage

        public Instruction(String instruction, int number) {

        }

    }

    public class DataLocation { // register or memory

        int type; // 0 = register, 1 = memory
        String register;
        int offset;

        public DataLocation(String register) { // register
            this.type = 0;
            this.register = register;
            this.offset = 0;
        }

        public DataLocation(String register, int offset) { // memory location
            this.type = 1;
            this.register = register;
            this.offset = offset;
        }

        public override bool Equals(object obj) {
            DataLocation other = (DataLocation) obj;
            return (this.register.Equals(other.register)) && (this.offset == other.offset);
        }
    }

    
















    public class Pipeline {

        List<String> pendingWrites; // registers or memory locations to be overwritten
        List<InstructionStages> pipeline; // holds pipeline
        int clockCycle;             // current clock cycle

        public Pipeline(String[] instructions, int pipelineType) {
            pendingWrites = new List<String>();
            pipeline = new List<InstructionStages>();
            clockCycle = 1;
        }

    }

    public class InstructionStages {



    }

    public struct Stage {

        int clock; // current clock cycle
        int stage; // 1=IF, 2=ID, 3=EX, 4=MEM, 5=WB
        bool resultWritten; // true if result has been written and is available
        String result; // register or memory location to hold result
        String operand1; // register or memory location
        String operand2; // register or memory location

    }
}
