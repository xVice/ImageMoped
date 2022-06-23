namespace ImageMoped
{
    public partial class Form1 : Form
    {
        private static Bitmap operationImage;
        private BitMapBender bmb;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            operationImage = default(Bitmap);
            bmb = new BitMapBender(operationImage);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fileName;
                fileName = dlg.FileName;
                operationImage = (Bitmap)Image.FromFile(fileName);
                bmb.SetBitmap(operationImage);
                pictureBox1.Image = operationImage;
            }
            await bmb.LoadIntoPixelList();
            List<BitMapBender.Pixel> pixelList = bmb.GetPixelList();
            listBox1.Items.AddRange(bmb.GetPixelStrings().ToArray());
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await bmb.FlipShit();
            pictureBox1.Image = (Image)bmb.GetBitMap();
        }
    }





    public class BitMapBender
    {
        private Bitmap bitmap;
        private List<Pixel> pixelList;
        private List<string> pixelstrings;

        public BitMapBender(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            this.pixelList = new List<Pixel>();
            this.pixelstrings = new List<string>();
        }

        public void SetBitmap(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public Bitmap GetBitMap()
        {
            return bitmap;
        }

        public async Task<Task> FlipShit()
        {
            List<Pixel> oldList = pixelList;
            List<Pixel> newList = new List<Pixel>();
            newList.Capacity = oldList.Capacity;
            int maxY = bitmap.Height;
            int maxX = bitmap.Width ;
            foreach (Pixel pixel in oldList)
            {
                if(maxX == 0) { maxX = bitmap.Width; }
                var curPixel = pixelList[pixel.index];
                curPixel.x = maxX;
                maxX--;
                newList.Add(curPixel);
            }
            Bitmap bmp = await ListToBitMap(newList);
            bmp.Save("./Flipperd.bmp");
            return Task.CompletedTask;
        }

        public Task<Bitmap> ListToBitMap(List<Pixel> list)
        {
            foreach(var pixel in list)
            {
                bitmap.SetPixel(pixel.x, pixel.y, Color.FromArgb(pixel.r, pixel.g, pixel.b));
            }
            return (Task<Bitmap>)Task.CompletedTask;
        }

        public Task LoadIntoPixelList()
        {
            pixelList.Clear();
            pixelstrings.Clear();
            pixelList.Capacity = bitmap.Width * bitmap.Height;
            int ind = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var currentPixel = new Pixel();
                    currentPixel.index = ind;
                    ind++;
                    currentPixel.x = x;
                    currentPixel.y = y;
                    currentPixel.r = bitmap.GetPixel(currentPixel.x, currentPixel.y).R;
                    currentPixel.g = bitmap.GetPixel(currentPixel.x, currentPixel.y).G;
                    currentPixel.b = bitmap.GetPixel(currentPixel.x, currentPixel.y).B;
                    Console.WriteLine(currentPixel.ToString());

                    pixelstrings.Add(currentPixel.ToString());
                    pixelList.Add(currentPixel);
                }
            }
            Console.WriteLine($"{pixelList.Count} Pixels have been added to the pixelist!");
            Console.WriteLine($"Loading UI this will take some time!");
            return Task.CompletedTask;
        }

        public List<string> GetPixelStrings()
        {
            return pixelstrings;
        }

        public List<Pixel> GetPixelList()
        {
            return pixelList;
        }

        public struct Pixel
        {
            public int index { get; set; }
            public int x { get; set; }
            public int y { get; set; }
            public byte r { get; set; }
            public byte g { get; set; }
            public byte b { get; set; }

            public override string ToString()
            {
                return $"Pixel:XY[{x},{y}]RGB:[{r},{g},{b}]";
            }
        }
    }
}