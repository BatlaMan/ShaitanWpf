using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaitanWpf
{
    public interface IFileDragDropTarget
    {
        void OnFileDrop(string[] filepaths);
    }
}
