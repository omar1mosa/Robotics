using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace testing_robotics
{
    public partial class Form1 : Form
    {
        int   CellSize = 31,
              
        //بداية الذراع على المحاور
              StartingCellX = 9,
              StartingCellY = 5;

        int   StartingCordinatesX,
              StartingCordinatesY;
        float StepRotateAngleL1 = 0,
              OldRotateAngleL1 = 0,

              StepRotateAngleL2 = 0,
              OldRotateAngleL2 = 0;

        bool  AreValidInputs = false;

        float AngleTargetL1 = 0,
              AngleTargetL2 = 0,
              Step = 0.4f;

        int numCellsY = 0;
        int numCellsX = 0;

        Point startPointL1;
        Point endPointL1;

        Point startPointL2;
        Point endPointL2;
        
        int LengthL1 = 15, LengthL2 = 13;

        Font MyFont = new Font("Arial", 10);

        public Form1()
        {
            InitializeComponent();
            
            StartingCordinatesX = StartingCellX * CellSize;
            StartingCordinatesY = StartingCellY * CellSize;

            labelX.BackColor = ColorTranslator.FromHtml("blue");
            labelY.BackColor = ColorTranslator.FromHtml("blue");
            labelX.Font = new Font(labelX.Font, FontStyle.Bold);
            labelY.Font = new Font(labelY.Font, FontStyle.Bold);

            labelTheta1.BackColor = ColorTranslator.FromHtml("green");
            labelTheta2.BackColor = ColorTranslator.FromHtml("green");
            labelTheta1.Font = new Font(labelX.Font, FontStyle.Bold);
            labelTheta2.Font = new Font(labelY.Font, FontStyle.Bold);

        }

        private void labelX_Click(object sender, EventArgs e)
        {

        }
        //الشبكة 
        private void Form1_Load(object sender, EventArgs e)
        {
            
            pictureBox1.BackColor = Color.White;
            pictureBox1.BorderStyle = BorderStyle.None;
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
           
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            numCellsX = (pictureBox1.Width - StartingCordinatesX) / CellSize;
            numCellsY = (pictureBox1.Height - StartingCordinatesY) / CellSize + 1;

            // لون خطوط الشبكة
            Pen gridPen = new Pen(Color.Red);
            gridPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            // لون وحجم محاور الاحداثيات
            Pen axisPen = new Pen(Color.Black, 5);



            // تحديد خصائص الشبكة

            // رسم الشبكة
            for (int x = StartingCellX; x <= numCellsX + StartingCordinatesX; x++)
            {
                int xPos = x * CellSize;
                e.Graphics.DrawLine(gridPen, xPos, 0, xPos, pictureBox1.Height - (StartingCellY * CellSize));


            } /// Y محور

            for (int y = 0; y <= numCellsY; y++)
            {
                int yPos = y * CellSize;
                e.Graphics.DrawLine(gridPen, StartingCellX * CellSize, yPos, pictureBox1.Width, yPos);

            } //// X محور

            // رسم المحاور
            e.Graphics.DrawLine(axisPen, StartingCordinatesX, pictureBox1.Height - StartingCordinatesY
                                        , pictureBox1.Width, pictureBox1.Height - StartingCordinatesY); // X-محور

            e.Graphics.DrawLine(axisPen, StartingCordinatesX         , pictureBox1.Height - StartingCordinatesY  
                                       , StartingCellX * CellSize    , 0); // Y-محور

            //لون الخط لنهاية النمط النقطي وحجمه
            Pen pen = new Pen(ColorTranslator.FromHtml("#00B050"), 3);
            pen.DashStyle = DashStyle.Dot; // تحديد النمط النقطي للخط
            //المفصل الأول
            DrawCircleEllipse(e.Graphics, pen, StartingCordinatesX, numCellsY * CellSize, 29 * CellSize);
            DrawCircleFill(e.Graphics, ColorTranslator.FromHtml("Blue"), StartingCordinatesX, numCellsY * CellSize, 26);
            drawRobotArm(e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (AreValidInputs)
            {
                if(StepRotateAngleL1 > 0)
                {

                }
                var rAngleTargetL1 = Math.Round(AngleTargetL1);
                var rOldRotateAngleL1 = Math.Round(OldRotateAngleL1);
                
                var rAngleTargetL2 = Math.Round(AngleTargetL2);
                var rOldRotateAngleL2 = Math.Round(OldRotateAngleL2);

                if (rAngleTargetL1 == rOldRotateAngleL1)
                    StepRotateAngleL1 = 0;


                var absSubL2 = Math.Abs(OldRotateAngleL2 - AngleTargetL2);

                if (rAngleTargetL2 == rOldRotateAngleL2)
                    StepRotateAngleL2 = 0;

                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            float x = float.Parse(labelX.Text);
            float y = float.Parse(labelY.Text);

            bool isOutSideOuterCircle = (Math.Pow(x, 2) + Math.Pow(y, 2)) > Math.Pow(LengthL1 + LengthL2, 2);
            bool isInSideInnerCircle = (Math.Pow(x, 2) + Math.Pow(y, 2)) < Math.Pow(LengthL1 - LengthL2, 2);

            if ( x < 0 || y < 0 ||  isOutSideOuterCircle || isInSideInnerCircle)
            {
                MessageBox.Show("التحديد خارج المنطقة المسموح بها");
                return;
            }

            double theta2 = Math.Acos((Math.Pow(x, 2) + Math.Pow(y, 2) - Math.Pow(LengthL1, 2) - Math.Pow(LengthL2, 2)) / (2 * LengthL1 * LengthL2));

            double theta1 = (Math.Atan(y / x)) - (Math.Asin((LengthL2 * Math.Sin(theta2))/ (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)))));


            theta1 = theta1 * (180.0 / Math.PI) - OldRotateAngleL1; // تحويل الزاوية إلى الدرجات
            theta2 = theta2 * (180.0 / Math.PI) - OldRotateAngleL2;

            labelTheta1.Text = theta1.ToString("F3");
            labelTheta2.Text = theta2.ToString("F3");

            AreValidInputs = true;
            AngleTargetL1 = OldRotateAngleL1 + (float)theta1;
            AngleTargetL2 = OldRotateAngleL2 + (float)theta2;
            //تحديد اتجاه حركة الذراع
            if (theta1 > 0)
                StepRotateAngleL1 = Step;
            else
                StepRotateAngleL1 = -Step;


            if (theta2 > 0)
                StepRotateAngleL2 = Step;
            else
                StepRotateAngleL2 = -Step;

        }
        //تحديد مكان المؤشر
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            float x = (e.X / CellSize)- StartingCellX +0.5f;
            float y = ((pictureBox1.Height- e.Y) / CellSize)-StartingCellY + 0.5f;

            labelX.Text = x.ToString();
            labelY.Text = y.ToString();
        }
        //رسم الذراع
        private void drawRobotArm(PaintEventArgs e)
        {
            startPointL1 = new Point(StartingCordinatesX, pictureBox1.Height - StartingCordinatesY);
            endPointL1 = new Point((LengthL1 * CellSize) + StartingCordinatesX, pictureBox1.Height - StartingCordinatesY);

            startPointL2 = endPointL1;
            endPointL2 = new Point(startPointL2.X + (LengthL2 * CellSize), pictureBox1.Height - StartingCordinatesY);

            e.Graphics.TranslateTransform(StartingCordinatesX, numCellsY*CellSize);
            e.Graphics.RotateTransform(-OldRotateAngleL1);
            e.Graphics.RotateTransform(-StepRotateAngleL1);
            e.Graphics.TranslateTransform(-StartingCordinatesX, -numCellsY * CellSize);
            //الذراع الأول 
            Pen pen = new Pen(ColorTranslator.FromHtml("Blue"), 15);

            
            DrawLine(e.Graphics, pen, startPointL1, endPointL1);
            // لون المفصل الثاني
            DrawCircleFill(e.Graphics, ColorTranslator.FromHtml("#F4B183"), endPointL1.X, endPointL1.Y, 15);

            OldRotateAngleL1 += StepRotateAngleL1;

            e.Graphics.TranslateTransform(startPointL2.X, numCellsY * CellSize);
            e.Graphics.RotateTransform(-OldRotateAngleL2);
            e.Graphics.RotateTransform(-StepRotateAngleL2);
            e.Graphics.TranslateTransform(-startPointL2.X, -numCellsY * CellSize);
            // الذراع الثاني
            pen = new Pen(ColorTranslator.FromHtml("#F4B183"), 10);
            

            DrawLine(e.Graphics, pen, startPointL2, endPointL2);

            OldRotateAngleL2 += StepRotateAngleL2;

            // رسم وتدوير المستطيل
            

            pen = new Pen(ColorTranslator.FromHtml("#FF0000"), 10);

            // لون نهاية الذراع 
            DrawCircleFill(e.Graphics, ColorTranslator.FromHtml("red"), endPointL2.X, endPointL2.Y, 10);

        }

        private void DrawCircleFill(Graphics g, Color color, int x, int y, int radius)
        {
            // إنشاء SolidBrush لتعيين لون الدائرة
            SolidBrush brush = new SolidBrush(color);

            //   رسم المفااصل
            g.FillEllipse(brush, x - radius, y - radius, radius * 2, radius * 2);

            // تحرير موارد SolidBrush
            brush.Dispose();
        }

        private void DrawCircleEllipse(Graphics g, Pen pen, int x, int y, int radius)
        {
            // رسم دائرة بلون حواف فقط
            g.DrawEllipse(pen, x - radius, y - radius, radius * 2, radius * 2);
        }

        private void DrawLine(Graphics g, Pen pen, Point p1, Point p2)
        {
            // رسم خط بين النقطتين p1 و p2 باستخدام القلم المعطى
            g.DrawLine(pen, p1, p2);
        }
    }
}
