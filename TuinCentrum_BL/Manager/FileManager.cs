using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuinCentrum_BL.Interfaces;

namespace TuinCentrum_BL.Manager
{
        public class FileManager
        {
            private IFileProcessor _fileProcessor;
            private ITuinCentrumRepository _tuinCentrumRepository;
        public FileManager(IFileProcessor fileProcessor, ITuinCentrumRepository tuinCentrumRepository)
        {
            _fileProcessor = fileProcessor;
            _tuinCentrumRepository = tuinCentrumRepository;
        }
        public void 
        }
    
}
