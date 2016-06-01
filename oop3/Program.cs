using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace oop3
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Ellipse el = new Ellipse(new Point(55, 35), 20, 5, Color.Red);
            Circle circle = new Circle(new Point(55, 55), 10);
            Circle circle2 = new Circle(new Point(15, 55), 5);
            FilledCircle fcircle = new FilledCircle(new Point(55, 35), 10, Color.Blue, Color.Red);
            Cone cone = new Cone(fcircle, 20);

            Frustum frustum = new Frustum(circle, 20, 10);

            Image image1 = new Image(frustum, el, circle2, circle, fcircle, cone, frustum);
            Image image2 = new Image(frustum, el, fcircle, cone, frustum);
            Image image3 = new Image(el, fcircle, cone);

           // frustum.Move(0, 10);

           // frustum.Move(10, 0);

            int a = MyArea.Area(image1);

            Console.WriteLine(a);

            image1.Draw();
            image1.SaveToFile("image.txt");
        }
    }
}
