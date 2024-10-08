using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Drawing.Drawing2D;

namespace CalculatorProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void SetRoundedCornersToForm()
        {
            int radius = 30; // نصف قطر الزوايا الدائرية
            GraphicsPath path = new GraphicsPath();

            // إضافة مستطيل بزوايا دائرية إلى المسار
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(this.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, this.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();

            this.Region = new Region(path); // تعيين المنطقة المحددة للمسار
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetRoundedCornersToForm(); // إعادة تعيين الزوايا عند تغيير حجم النموذج

        }
        public  void helper(Char Operation, double Number, Stack<Object> helperStack)
        {
            if (Operation == '+')
                helperStack.Push(Number);

            else if (Operation == '-')
                helperStack.Push(-Number);

            else if (Operation == '*')
                helperStack.Push((double)helperStack.Pop() * Number);

            else if (Operation == '/')
                helperStack.Push((double)((double)helperStack.Pop() / Number));

        }
        public  double calculate(string Text)
        {
            if (Text.Length == 0) return 0;

            char Operation = '+';
            double currentNumber = 0;
            bool isDecimal = false;
            double decimalPlaceValue = 1;
            double sum = 0;

            Stack<Object> helperStack = new Stack<Object>();

            for (int i = 0; i < Text.Length; i++)
            {

                if (Char.IsDigit(Text[i]))
                {
                    double digit = Text[i] - '0';

                    if (isDecimal)
                    {
                        decimalPlaceValue *= 10;
                        currentNumber += (double)digit / decimalPlaceValue;
                    }
                    else
                    {
                        currentNumber = (currentNumber * 10) + digit;
                    }
                }


                if (!Char.IsDigit(Text[i]))
                {
                    if (Text[i] == '.')
                    {
                        isDecimal = true;
                        continue;
                    }

                    else if (Text[i] == '(')
                    {
                        helperStack.Push(Operation);
                        currentNumber = 0;
                        Operation = '+';
                        continue;
                    }

                    else if (Text[i] == '+' || Text[i] == '-' || Text[i] == '*' || Text[i] == '/' || Text[i] == ')')
                    {
                        helper(Operation, currentNumber, helperStack);
                    }

                    if (Text[i] == ')')
                    {
                        currentNumber = 0;
                        while (double.TryParse(Convert.ToString(helperStack.Peek()), out _))
                        {
                            currentNumber += (double)helperStack.Pop();
                        }
                        Operation = (char)helperStack.Pop();
                        helper(Operation, currentNumber, helperStack);

                    }

                    currentNumber = 0;
                    Operation = Text[i];
                    isDecimal = false;
                    decimalPlaceValue = 1;
                }
            }

            helper(Operation, currentNumber, helperStack);

            while (helperStack.Count > 0)
            {
                sum += (double)helperStack.Pop();
            }

            return sum;
        }
        private bool CheckIfIsWarnings(Button button)
        {
            if (txtArthmaticOperation.Text == string.Empty && button.Tag.ToString() == "0")
            {
                return true;
            }
            else if (txtArthmaticOperation.Text != string.Empty)
            {

                
                if(txtArthmaticOperation.Text[txtArthmaticOperation.Text.Length - 1] == '0'
                    && Char.IsDigit(Convert.ToChar(button.Tag)) 
                    && !Char.IsNumber(txtArthmaticOperation.Text[txtArthmaticOperation.Text.Length - 2]))
                return true;
            }

           return false;
        }

        private void UpdateArthmaticOperation(Button button)
        {
            if(CheckIfIsWarnings(button))
            {
                return;
            }

            txtArthmaticOperation.Text += button.Tag.ToString();
        }
        private void button_Click(object sender, EventArgs e)
        {
            UpdateArthmaticOperation((Button )sender);
        }

       
        private void ClearText_Click(object sender, EventArgs e)
        {
            txtArthmaticOperation.Text = "";
            lbResult.Text = "0";
            
        }

       
        private void Delete_Click(object sender, EventArgs e)
        {
            if (txtArthmaticOperation.Text != string.Empty)
            {
                txtArthmaticOperation.Text = txtArthmaticOperation.Text.Substring(0, txtArthmaticOperation.Text.Length - 1);
            }
        }

        private void SaveResultInRegister(double Result)
        {
            
            StreamWriter streamWriter = new StreamWriter(@"C:\Users\ahmad\OneDrive\Desktop\My Projects\Project Course 14\CalculatorProject\Register.txt", true);
            streamWriter.WriteLine(DateTime.Now.ToString());
            streamWriter.WriteLine(txtArthmaticOperation.Text +" = "+Result.ToString()+"\n");
            streamWriter.Close();
        }

        private void ReadRegister()
        {

            txtRegister.Text = "";
            txtRegister.TextAlign = HorizontalAlignment.Left;
            StreamReader streamReader = new StreamReader(@"C:\Users\ahmad\OneDrive\Desktop\My Projects\Project Course 14\CalculatorProject\Register.txt");

            string line = "";

            while((line = streamReader.ReadLine()) != null)
            {
                txtRegister.Text += (line) + Environment.NewLine;
            }

            streamReader.Close();
        }
        private void ShowEndResult_Click(object sender, EventArgs e)
        {
            if (txtArthmaticOperation.Text != string.Empty)
            {
               double Result =  calculate(txtArthmaticOperation.Text);
               lbResult.Text = Convert.ToString(Result);
               SaveResultInRegister(Result);
               ReadRegister();
            }
            else
            {
                MessageBox.Show("there is no Number to Implement Opeartion for it","Warn",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            ReadRegister();

            if(txtRegister.Text ==string.Empty)
            {
                txtRegister.TextAlign= HorizontalAlignment.Center;
                txtRegister.Text = "There is no Register ";
            }
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StreamWriter streamWriter = new StreamWriter(@"C:\Users\ahmad\OneDrive\Desktop\My Projects\Project Course 14\CalculatorProject\Register.txt");
            streamWriter.Flush();
            streamWriter.Close();
            txtRegister.TextAlign = HorizontalAlignment.Center;
            txtRegister.Text = "There is no Register ";
        }
    }
}
