using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DMX512Reciever.Common;
using DMX512Reciever.Model;

namespace DMX512Reciever.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        public DMX512 DMX512Buffer = new DMX512();

        private const int dmxAddress = 170;

        private Brush[] _rectFillColor = new Brush[dmxAddress];
        public Brush[] RectFillColor
        {
            get { return _rectFillColor; }
            set { _rectFillColor = value; }
        }

        public MainWindowViewModel()
        {
            for(int x = 0; x < dmxAddress; x++)
            {
                RectFillColor[x] = new SolidColorBrush(Color.FromArgb((byte)255, (byte)0, (byte)0, (byte)0));
            }
        }
    }
}
