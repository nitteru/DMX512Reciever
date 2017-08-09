using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMX512Reciever.Model
{
    public class DMX512
    {
        private const int channel = 512;
        private const int dmxAddress = 170;

        private byte _startCode = 0;
        public byte StartCode
        {
            get { return _startCode; }
            set { _startCode = value; }
        }

        private byte[] _dmxR = new byte[channel];
        public byte[] DMXR
        {
            get { return _dmxR; }
            set { _dmxR = value; }
        }

        private byte[] _dmxG = new byte[channel];
        public byte[] DMXG
        {
            get { return _dmxG; }
            set { _dmxG = value; }
        }

        private byte[] _dmxB = new byte[channel];
        public byte[] DMXB
        {
            get { return _dmxB; }
            set { _dmxB = value; }
        }

        public DMX512()
        {
            StartCode = 0;

            for(int i = 0; i < channel; i++)
            {
                DMXR[i] = 0;
                DMXG[i] = 0;
                DMXB[i] = 0;
            }
        }
        
    }
}
