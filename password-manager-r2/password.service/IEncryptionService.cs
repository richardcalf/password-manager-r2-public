﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password.service
{
    public interface IEncryptionService
    {
        string Decrypt(string value);
        string Encrypt(string value);
    }
}
