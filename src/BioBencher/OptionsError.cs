using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioBencher
{
    public enum OptionsError
    {
        InvalidFile,

        InvalidFileFormat,

        UnparseableCommand,

        FileDoesNotExist,

        InvalidSequenceDefinitionError,

        CommandFailed,
        UnknownError,
        InvalidTaskId,
    }
}
