using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PipelineHazardDetector {

    enum DataType { register, memory }
    enum InstructionType { invalid, add, sub, load, store }
    enum PipelineStage { STALL, IF, ID, EX, MEM, WB }
    //enum Registers { zero, at, v0, v1, a0, a1, a2, a3, t0, t1, t2, t3, t4, t5, t6, t7, s0, s1, s2, s3, s4, s5, s6, s7, t8, t9, k0, k1, gp, fp, ra }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        // pipelineType: 1 = with hazards, 2 = without forwarding, 3 = with forwarding
        public static void ParseInstructions(String input, int pipelineType) {

            // Separate each instruction
            String[] instructionArray = input.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

            List<Instruction> instructions = new List<Instruction>(); // holds parsed instructions
            List<DataDependence> dataDependences = new List<DataDependence>(); // holds list of data dependences

            // create instructions and put them in the list
            for (int i = 0; i < instructionArray.Length; i++) {
                Instruction currentInstruction = new Instruction(instructionArray[i], i);
                if ((InstructionType) currentInstruction.GetInstructionType() == InstructionType.invalid) {
                    continue; // skip the invalid instruction
                }
                // check previous instructions for data dependences
                bool source1Dependence = false;
                bool source2Dependence = false;
                for (int j = instructions.Count - 1; j >= 0; j--) {
                    if (!source1Dependence && instructions[j].GetDestination().Equals(currentInstruction.GetSource1())) {
                        dataDependences.Add(new DataDependence(instructions[j].GetInstructionNumber(), i, 1));
                        source1Dependence = true;
                    }
                    if (!source2Dependence && instructions[j].GetDestination().Equals(currentInstruction.GetSource2())) {
                        dataDependences.Add(new DataDependence(instructions[j].GetInstructionNumber(), i, 2));
                        source2Dependence = true;
                    }
                }
                instructions.Add(currentInstruction); // add instruction to parsed instructions list
            }



        }
    }

    public class Instruction {

        int instructionNumber;              // which instruction it is, in written order
        InstructionType instructionType;    // type of instruction: add, sub, load, or store
        DataLocation source1;               // source operand 1
        DataLocation source2;               // source operand 2 (if applicable)
        DataLocation destination;           // register or memory location to write to

        public Instruction(String instruction, int number) {

            String[] operands;

            this.instructionNumber = number;

            instruction.Trim(); // remove whitespace at beginning and end
            instruction = this.ParseInstructionType(instruction); // set instruction type

            if (this.instructionType == InstructionType.invalid) {
                return;
            }

            operands = instruction.Split(',');

            // validate number of operands
            if (instructionType == InstructionType.add || instructionType == InstructionType.sub) {
                if (operands.Length != 3) {
                    instructionType = InstructionType.invalid;
                    return;
                }
            } else if (instructionType == InstructionType.load || instructionType == InstructionType.store) {
                if (operands.Length != 2) {
                    instructionType = InstructionType.invalid;
                    return;
                }
            }

            this.ParseOperands(operands); // set operands

        }

        private String ParseInstructionType(String instruction) {

            String typeString = "";
            
            // get the first word in the instruction
            foreach (char character in instruction) {
                if (Char.IsWhiteSpace(character)) {
                    break;
                }
                typeString = typeString + character;
            }

            // set the type
            if (typeString.Equals("add")) {
                this.instructionType = InstructionType.add;
            } else if (typeString.Equals("sub")) {
                this.instructionType = InstructionType.sub;
            } else if (typeString.Equals("lw")) {
                this.instructionType = InstructionType.load;
            } else if (typeString.Equals("sw")) {
                this.instructionType = InstructionType.store;
            } else {
                this.instructionType = InstructionType.invalid;
            }

            // return the instruction with type removed
            return instruction.Substring(typeString.Length).Trim();
        }

        // TODO: validate register values & offset values
        private void ParseOperands(String[] operands) {

            this.destination = new DataLocation(operands[0].Trim());

            if (operands.Length == 2) {
                String[] memoryLocation;
                memoryLocation = operands[1].Split(new[] {'(', ')'}, StringSplitOptions.RemoveEmptyEntries);
                if (memoryLocation.Length != 2) {
                    this.instructionType = InstructionType.invalid;
                    return;
                }
                this.source1 = new DataLocation(memoryLocation[1].Trim(), memoryLocation[0].Trim());
                this.source2 = new DataLocation("0");
            } else if (operands.Length == 3) {
                this.source1 = new DataLocation(operands[1].Trim());
                this.source2 = new DataLocation(operands[2].Trim());
            }

        }
        
        public int GetInstructionNumber() {
            return this.instructionNumber;
        }

        public Enum GetInstructionType() {
            return this.instructionType;
        }

        public DataLocation GetDestination() {
            return this.destination;
        }

        public DataLocation GetSource1() {
            return this.source1;
        }

        public DataLocation GetSource2() {
            return this.source2;
        }

    }

    public class DataLocation { // register or memory

        DataType type;
        String register;
        String offset;

        public DataLocation(String register) { // register
            this.type = DataType.register;
            this.register = register;
            this.offset = "N/A";
        }

        public DataLocation(String register, String offset) { // memory location
            this.type = DataType.memory;
            this.register = register;
            this.offset = offset;
        }

        public override bool Equals(object obj) {
            DataLocation other = (DataLocation) obj;
            return (this.register.Equals(other.register)) && (this.offset == other.offset);
        }
    }

    public class DataDependence {

        int earlierInstructionNumber;
        int laterInstructionNumber;
        int sourceRegisterNumber;

        public DataDependence(int earlierInstructionNumber, int laterInstructionNumber, int sourceRegisterNumber) {
            this.earlierInstructionNumber = earlierInstructionNumber;
            this.laterInstructionNumber = laterInstructionNumber;
            this.sourceRegisterNumber = sourceRegisterNumber;
        }

    }

    public class Pipeline {

        int[] instruction1 = {0, 0, 0, 0, 0};
        int[] instruction2 = {0, 0, 0, 0, 0};
        int[] instruction3 = {0, 0, 0, 0, 0};
        int[] instruction4 = {0, 0, 0, 0, 0};
        int[] instruction5 = {0, 0, 0, 0, 0};
        int[] instruction6 = {0, 0, 0, 0, 0};
        int[] instruction7 = {0, 0, 0, 0, 0};

        public Pipeline(List<Instruction> instructions) {

        }

    }

}
