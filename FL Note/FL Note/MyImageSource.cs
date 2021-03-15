using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FL_Note
{
	public class MyImageSource : StreamImageSource
    {
        public byte[] ImageBytes { get; private set; } = null;
        public static MyImageSource FromBytes(byte[] data)
        {
            Stream stream = new MemoryStream(data);
            MyImageSource imageSource = (MyImageSource)FromStream(() => stream);
            imageSource.ImageBytes = data;
            return imageSource;
        }

        public static new ImageSource FromStream(Func<Stream> stream)
        {
            return new MyImageSource
            {
                Stream = ((CancellationToken token) => Task.Run(stream, token))
            };
        }
    }
}