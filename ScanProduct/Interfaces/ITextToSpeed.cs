using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanProduct.TextToSpeedGoogle
{
    public interface ITextToSpeed
    {
        void  SpeedGoogle(string text);

        Task PlayMp3(string filePath);
    }
}
