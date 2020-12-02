using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PipelineHazardDetector {

    public enum DataType { register, memory }
    public enum InstructionType { invalid, add, sub, load, store }
    public enum PipelineStage { STALL, IF, ID, EX, MEM, WB }
    //enum Registers { zero, at, v0, v1, a0, a1, a2, a3, t0, t1, t2, t3, t4, t5, t6, t7, s0, s1, s2, s3, s4, s5, s6, s7, t8, t9, k0, k1, gp, fp, ra }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        // pipelineType: 1 = with hazards, 2 = without forwarding, 3 = with forwarding
        public static Pipeline ParseInstructions(String input, int pipelineType) {

            Pipeline pipeline;
            
            // Separate each instruction
            String[] instructionArray = input.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

            List<Instruction> instructions = new List<Instruction>(); // holds parsed instructions
            List<DataDependence> dataDependences = new List<DataDependence>(); // holds list of data dependences

            // create instructions and put them in the list
            for (int i = 0; i < instructionArray.Length; i++) {
                Instruction currentInstruction = new Instruction(instructionArray[i], i + 1);
                if ((InstructionType) currentInstruction.GetInstructionType() == InstructionType.invalid) {
                    continue; // skip the invalid instruction
                }
                // check previous instructions for data dependences
                bool source1Dependence = false;
                bool source2Dependence = false;
                for (int j = instructions.Count - 1; j >= 0; j--) {
                    if (!source1Dependence && instructions[j].GetDestination().Equals(currentInstruction.GetSource1())) {
                        dataDependences.Add(new DataDependence(pipelineType, (InstructionType) instructions[j].GetInstructionType(), (InstructionType) currentInstruction.GetInstructionType(), instructions[j].GetInstructionNumber(), i, 1));
                        source1Dependence = true;
                    }
                    if (!source2Dependence && instructions[j].GetDestination().Equals(currentInstruction.GetSource2())) {
                        dataDependences.Add(new DataDependence(pipelineType, (InstructionType) instructions[j].GetInstructionType(), (InstructionType) currentInstruction.GetInstructionType(), instructions[j].GetInstructionNumber(), i, 2));
                        source2Dependence = true;
                    }
                }
                instructions.Add(currentInstruction); // add instruction to parsed instructions list
            }

            pipeline = new Pipeline(pipelineType, instructionArray, instructions, dataDependences);

            return pipeline;
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
        InstructionType earlierInstructionType;
        InstructionType laterInstructionType;
        int sourceRegisterNumber;
        int dataAvailableAtStage;       // available at the end of this stage
        int dataNeededAtStage;          // needed at the beginning of this stage

        public DataDependence(int pipelineType, InstructionType earlierInstructionType, InstructionType laterInstructionType, int earlierInstructionNumber, int laterInstructionNumber, int sourceRegisterNumber) {
            this.earlierInstructionNumber = earlierInstructionNumber;
            this.laterInstructionNumber = laterInstructionNumber;
            this.earlierInstructionType = earlierInstructionType;
            this.laterInstructionType = laterInstructionType;
            this.sourceRegisterNumber = sourceRegisterNumber;

            if (earlierInstructionType == InstructionType.store) { // no hazards with store
                this.dataAvailableAtStage = 0;
                this.dataNeededAtStage = 0;
            } else if (pipelineType == 1 || pipelineType == 2) { // display hazards without fixing them, or pipeline with no forwarding unit
                this.dataAvailableAtStage = 5;
                this.dataNeededAtStage = 2;
            } else if (pipelineType == 3) { // pipeline with forwarding unit
                if (earlierInstructionType == InstructionType.load) {
                    this.dataAvailableAtStage = 4;
                } else { // add or sub
                    this.dataAvailableAtStage = 3;
                }
                this.dataNeededAtStage = 3;
            }
        }

        public int GetEarlierInstructionNumber() {
            return this.earlierInstructionNumber;
        }

        public int GetLaterInstructionNumber() {
            return this.laterInstructionNumber;
        }

        public InstructionType GetEarlierInstructionType() {
            return this.earlierInstructionType;
        }

        public InstructionType GetLaterInstructionType() {
            return this.laterInstructionType;
        }

        public int GetSourceRegisterNumber() {
            return this.sourceRegisterNumber;
        }

        public int GetDataAvailableAtStage() {
            return this.dataAvailableAtStage;
        }

        public int GetDataNeededAtStage() {
            return this.dataNeededAtStage;
        }

        public override bool Equals(object obj) {
            DataDependence other = (DataDependence) obj;
            return (this.earlierInstructionNumber == other.earlierInstructionNumber) && (this.laterInstructionNumber == other.laterInstructionNumber)
                && (this.sourceRegisterNumber == other.sourceRegisterNumber) && (this.dataAvailableAtStage == other.dataAvailableAtStage)
                && (this.dataNeededAtStage == other.dataNeededAtStage);
        }

    }

    public class Pipeline {

        String[] instructionArray;
        int[,] pipelinedInstructions = new int[7, 5];
        List<DataDependence> dataHazards;

        public Pipeline(int pipelineType, String[] instructionArray, List<Instruction> instructions, List<DataDependence> dataDependences) {

            int instructionStart;

            this.instructionArray = instructionArray;

            if (pipelineType == 1) {
                instructionStart = 0;
                for (int i = 0; i < 7; i++) {
                    instructionStart++;
                    if (i < instructions.Count) {
                        for (int j = 0; j < 5; j++) {
                            pipelinedInstructions[i, j] = instructionStart + j;
                        }
                    }
                    else {
                        for (int j = 0; j < 5; j++) {
                            pipelinedInstructions[i, j] = 0;
                        }
                    }
                }
                this.SetDataHazards(dataDependences);
            } else if (pipelineType == 2) {

            } else if (pipelineType == 3) {

            }

        }

        private void SetDataHazards(List<DataDependence> dataDependences) {

            List<DataDependence> falseDependences = new List<DataDependence>();

            foreach (DataDependence dataDependence in dataDependences) {
                Console.WriteLine("Data Dependence Earlier Instruction: " + dataDependence.GetEarlierInstructionNumber() + ", " + dataDependence.GetDataAvailableAtStage());
                Console.WriteLine("Data Dependence Later Instruction: " + dataDependence.GetLaterInstructionNumber() + ", " + dataDependence.GetDataNeededAtStage());
                int dataAvailableAtEndOfClockCycle = pipelinedInstructions[dataDependence.GetEarlierInstructionNumber() - 1, dataDependence.GetDataAvailableAtStage() - 1];
                int dataNeededAtBeginningOfClockCycle = pipelinedInstructions[dataDependence.GetLaterInstructionNumber() - 1, dataDependence.GetDataNeededAtStage() - 1];
                Console.WriteLine(dataNeededAtBeginningOfClockCycle + " > " + dataAvailableAtEndOfClockCycle + " ?");
                if (dataNeededAtBeginningOfClockCycle > dataAvailableAtEndOfClockCycle) {
                    falseDependences.Add(dataDependence);
                }
            }

            foreach (DataDependence falseDependence in falseDependences) {
                dataDependences.Remove(falseDependence);
            }

            this.dataHazards = dataDependences;
        }

        public String[] GetInstructionArray() {
            return this.instructionArray;
        }

        public int[,] GetPipelinedInstructions() {
            return pipelinedInstructions;
        }

        public List<DataDependence> GetDataHazards() {
            return this.dataHazards;
        }
    }

}
