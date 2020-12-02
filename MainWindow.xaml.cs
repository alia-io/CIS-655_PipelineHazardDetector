using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PipelineHazardDetector {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private List<Label> labels = new List<Label>();
        private List<Path> arrows = new List<Path>();

        public MainWindow() {
            InitializeComponent();
            InitializeLabelsList();
            InitializeArrowsList();
        }

        private void DisplayWithHazardsOnClick(object sender, RoutedEventArgs e) {
            String instructionSequence = InstructionSequence.Text;
            Pipeline pipeline = App.ParseInstructions(instructionSequence, 1);
            String[] instructionArray = pipeline.GetInstructionArray();
            int[,] pipelinedInstructions = pipeline.GetPipelinedInstructions();
            List<DataDependence> dataHazards = pipeline.GetDataHazards();

            // TODO: hide all arrows
            // TODO: remove all text from instructions & pipeline area

            for (int i = 0; i < 7; i++) {

                if (i < instructionArray.Length) {
                    switch (i) {
                        case 0: Instruction1.Content = instructionArray[i]; break;
                        case 1: Instruction2.Content = instructionArray[i]; break;
                        case 2: Instruction3.Content = instructionArray[i]; break;
                        case 3: Instruction4.Content = instructionArray[i]; break;
                        case 4: Instruction5.Content = instructionArray[i]; break;
                        case 5: Instruction6.Content = instructionArray[i]; break;
                        case 6: Instruction7.Content = instructionArray[i]; break;
                    }
                }

                for (int j = 0; j < 5; j++) {
                    if (pipelinedInstructions[i, j] != 0) {
                        Label label = FindLabel(ConvertToLabelName(i + 1, pipelinedInstructions[i, j]));
                        if (label != null) {
                            switch (j) {
                                case 0: label.Content = " IF"; break;
                                case 1: label.Content = "ID"; break;
                                case 2: label.Content = "EX"; break;
                                case 3: label.Content = " M"; break;
                                case 4: label.Content = "WB"; break;
                            }
                        }
                    }
                }

            }

            foreach (DataDependence dataHazard in dataHazards) {
                Path arrow = FindArrow(ConvertToArrowName(dataHazard.GetEarlierInstructionNumber(), dataHazard.GetLaterInstructionNumber(), dataHazard.GetEarlierInstructionType(), dataHazard.GetLaterInstructionType(), dataHazard.GetSourceRegisterNumber()));
                if (arrow != null) {
                    arrow.Visibility = Visibility.Visible;
                }
            }

        }

        private void DisplayWithoutForwardingOnClick(object sender, RoutedEventArgs e) {
            String instructionSequence = InstructionSequence.Text;
            App.ParseInstructions(instructionSequence, 2);
            //A01.Content = "Test";
            //TextBox textBox = new TextBox();
            //textBox.Text = "Test";
            //PipelineDisplay.Children.Add(textBox);
            //MessageBox.Show("Display Without Forwarding: " + instructionSequence);
        }

        private void DisplayWithForwardingOnClick(object sender, RoutedEventArgs e) {
            String instructionSequence = InstructionSequence.Text;
            App.ParseInstructions(instructionSequence, 3);
            //MessageBox.Show("Display With Forwarding: " + instructionSequence);
        }

        private String ConvertToLabelName(int instructionNumber, int cycleNumber) {

            String labelName = "";
            String cycleNumberString = cycleNumber.ToString();

            switch (instructionNumber) {
                case 1: labelName = labelName + "A"; break;
                case 2: labelName = labelName + "B"; break;
                case 3: labelName = labelName + "C"; break;
                case 4: labelName = labelName + "D"; break;
                case 5: labelName = labelName + "E"; break;
                case 6: labelName = labelName + "F"; break;
                case 7: labelName = labelName + "G"; break;
            }

            labelName = labelName + cycleNumber;

            return labelName;
        }

        private Label FindLabel(String name) {
            foreach (Label label in labels) {
                if (label.Name.Equals(name)) {
                    return label;
                }
            }
            return null;
        }

        private String ConvertToArrowName(int firstInstructionNumber, int secondInstructionNumber, InstructionType firstInstructionType, InstructionType secondInstructionType, int sourceNumber) {

            String arrowName = "";

            switch (firstInstructionType) {
                case InstructionType.load:
                    arrowName = arrowName + "L";
                    break;
                case InstructionType.add:
                case InstructionType.sub:
                    arrowName = arrowName + "R";
                    break;
            }

            arrowName = arrowName + firstInstructionNumber;

            switch (secondInstructionType) {
                case InstructionType.load:
                    arrowName = arrowName + "L" + secondInstructionNumber;
                    break;
                case InstructionType.store:
                    arrowName = arrowName + "S" + secondInstructionNumber;
                    break;
                case InstructionType.add:
                case InstructionType.sub:
                    arrowName = arrowName + "R" + secondInstructionNumber + "_" + sourceNumber;
                    break;
            }

            return arrowName;
        }

        private Path FindArrow(String name) {
            foreach (Path arrow in arrows) {
                if (arrow.Name.Equals(name)) {
                    return arrow;
                }
            }
            return null;
        }

        private void InitializeLabelsList() {
            labels.Add(A1);
            labels.Add(A2);
            labels.Add(A3);
            labels.Add(A4);
            labels.Add(A5);
            labels.Add(A6);
            labels.Add(A7);
            labels.Add(A8);
            labels.Add(A9);
            labels.Add(A10);
            labels.Add(A11);
            labels.Add(A12);
            labels.Add(A13);
            labels.Add(A14);
            labels.Add(A15);
            labels.Add(A16);
            labels.Add(A17);
            labels.Add(A18);
            labels.Add(A19);
            labels.Add(A20);
            labels.Add(A21);
            labels.Add(A22);
            labels.Add(A23);
            labels.Add(A24);

            labels.Add(B1);
            labels.Add(B2);
            labels.Add(B3);
            labels.Add(B4);
            labels.Add(B5);
            labels.Add(B6);
            labels.Add(B7);
            labels.Add(B8);
            labels.Add(B9);
            labels.Add(B10);
            labels.Add(B11);
            labels.Add(B12);
            labels.Add(B13);
            labels.Add(B14);
            labels.Add(B15);
            labels.Add(B16);
            labels.Add(B17);
            labels.Add(B18);
            labels.Add(B19);
            labels.Add(B20);
            labels.Add(B21);
            labels.Add(B22);
            labels.Add(B23);
            labels.Add(B24);

            labels.Add(C1);
            labels.Add(C2);
            labels.Add(C3);
            labels.Add(C4);
            labels.Add(C5);
            labels.Add(C6);
            labels.Add(C7);
            labels.Add(C8);
            labels.Add(C9);
            labels.Add(C10);
            labels.Add(C11);
            labels.Add(C12);
            labels.Add(C13);
            labels.Add(C14);
            labels.Add(C15);
            labels.Add(C16);
            labels.Add(C17);
            labels.Add(C18);
            labels.Add(C19);
            labels.Add(C20);
            labels.Add(C21);
            labels.Add(C22);
            labels.Add(C23);
            labels.Add(C24);

            labels.Add(D1);
            labels.Add(D2);
            labels.Add(D3);
            labels.Add(D4);
            labels.Add(D5);
            labels.Add(D6);
            labels.Add(D7);
            labels.Add(D8);
            labels.Add(D9);
            labels.Add(D10);
            labels.Add(D11);
            labels.Add(D12);
            labels.Add(D13);
            labels.Add(D14);
            labels.Add(D15);
            labels.Add(D16);
            labels.Add(D17);
            labels.Add(D18);
            labels.Add(D19);
            labels.Add(D20);
            labels.Add(D21);
            labels.Add(D22);
            labels.Add(D23);
            labels.Add(D24);

            labels.Add(E1);
            labels.Add(E2);
            labels.Add(E3);
            labels.Add(E4);
            labels.Add(E5);
            labels.Add(E6);
            labels.Add(E7);
            labels.Add(E8);
            labels.Add(E9);
            labels.Add(E10);
            labels.Add(E11);
            labels.Add(E12);
            labels.Add(E13);
            labels.Add(E14);
            labels.Add(E15);
            labels.Add(E16);
            labels.Add(E17);
            labels.Add(E18);
            labels.Add(E19);
            labels.Add(E20);
            labels.Add(E21);
            labels.Add(E22);
            labels.Add(E23);
            labels.Add(E24);

            labels.Add(F1);
            labels.Add(F2);
            labels.Add(F3);
            labels.Add(F4);
            labels.Add(F5);
            labels.Add(F6);
            labels.Add(F7);
            labels.Add(F8);
            labels.Add(F9);
            labels.Add(F10);
            labels.Add(F11);
            labels.Add(F12);
            labels.Add(F13);
            labels.Add(F14);
            labels.Add(F15);
            labels.Add(F16);
            labels.Add(F17);
            labels.Add(F18);
            labels.Add(F19);
            labels.Add(F20);
            labels.Add(F21);
            labels.Add(F22);
            labels.Add(F23);
            labels.Add(F24);

            labels.Add(G1);
            labels.Add(G2);
            labels.Add(G3);
            labels.Add(G4);
            labels.Add(G5);
            labels.Add(G6);
            labels.Add(G7);
            labels.Add(G8);
            labels.Add(G9);
            labels.Add(G10);
            labels.Add(G11);
            labels.Add(G12);
            labels.Add(G13);
            labels.Add(G14);
            labels.Add(G15);
            labels.Add(G16);
            labels.Add(G17);
            labels.Add(G18);
            labels.Add(G19);
            labels.Add(G20);
            labels.Add(G21);
            labels.Add(G22);
            labels.Add(G23);
            labels.Add(G24);
        }

        private void InitializeArrowsList() {
            arrows.Add(L1R2_1);
            arrows.Add(L1R3_1);
            arrows.Add(L1R2_2);
            arrows.Add(L1R3_2);
            arrows.Add(L1L2);
            arrows.Add(L1L3);
            arrows.Add(L1S2);
            arrows.Add(L1S3);
            arrows.Add(R1R2_1);
            arrows.Add(R1R3_1);
            arrows.Add(R1R2_2);
            arrows.Add(R1R3_2);
            arrows.Add(R1L2);
            arrows.Add(R1L3);
            arrows.Add(R1S2);
            arrows.Add(R1S3);

            arrows.Add(L2R3_1);
            arrows.Add(L2R4_1);
            arrows.Add(L2R3_2);
            arrows.Add(L2R4_2);
            arrows.Add(L2L3);
            arrows.Add(L2L4);
            arrows.Add(L2S3);
            arrows.Add(L2S4);
            arrows.Add(R2R3_1);
            arrows.Add(R2R4_1);
            arrows.Add(R2R3_2);
            arrows.Add(R2R4_2);
            arrows.Add(R2L3);
            arrows.Add(R2L4);
            arrows.Add(R2S3);
            arrows.Add(R2S4);

            arrows.Add(L3R4_1);
            arrows.Add(L3R5_1);
            arrows.Add(L3R4_2);
            arrows.Add(L3R5_2);
            arrows.Add(L3L4);
            arrows.Add(L3L5);
            arrows.Add(L3S4);
            arrows.Add(L3S5);
            arrows.Add(R3R4_1);
            arrows.Add(R3R5_1);
            arrows.Add(R3R4_2);
            arrows.Add(R3R5_2);
            arrows.Add(R3L4);
            arrows.Add(R3L5);
            arrows.Add(R3S4);
            arrows.Add(R3S5);

            arrows.Add(L4R5_1);
            arrows.Add(L4R6_1);
            arrows.Add(L4R5_2);
            arrows.Add(L4R6_2);
            arrows.Add(L4L5);
            arrows.Add(L4L6);
            arrows.Add(L4S5);
            arrows.Add(L4S6);
            arrows.Add(R4R5_1);
            arrows.Add(R4R6_1);
            arrows.Add(R4R5_2);
            arrows.Add(R4R6_2);
            arrows.Add(R4L5);
            arrows.Add(R4L6);
            arrows.Add(R4S5);
            arrows.Add(R4S6);

            arrows.Add(L5R6_1);
            arrows.Add(L5R7_1);
            arrows.Add(L5R6_2);
            arrows.Add(L5R7_2);
            arrows.Add(L5L6);
            arrows.Add(L5L7);
            arrows.Add(L5S6);
            arrows.Add(L5S7);
            arrows.Add(R5R6_1);
            arrows.Add(R5R7_1);
            arrows.Add(R5R6_2);
            arrows.Add(R5R7_2);
            arrows.Add(R5L6);
            arrows.Add(R5L7);
            arrows.Add(R5S6);
            arrows.Add(R5S7);

            arrows.Add(L6R7_1);
            arrows.Add(L6R7_2);
            arrows.Add(L6L7);
            arrows.Add(L6S7);
            arrows.Add(R6R7_1);
            arrows.Add(R6R7_2);
            arrows.Add(R6L7);
            arrows.Add(R6S7);
        }

    }
}
