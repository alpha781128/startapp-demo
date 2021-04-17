using System;
using System.Collections.Generic;
using System.Text;

namespace Startapp.Shared.ViewModels
{
    public class PictureVM
    {
        public Guid Id { get; set; }
        public string NSource { get; set; }
        public string MSource { get; set; }
        public string SSource { get; set; }
        public string Active { get; set; }
    }
}
