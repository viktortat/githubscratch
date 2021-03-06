﻿using System;
using System.IO;

namespace Ploeh.Samples.Encapsulation.CodeExamples
{
    public class FileStore
    {
        public FileStore(string workingDirectory)
        {
            if (workingDirectory == null)
                throw new ArgumentNullException("workingDirectory");
            if (!Directory.Exists(workingDirectory))
                throw new ArgumentException("Boo", "workingDirectory");

            this.WorkingDirectory = workingDirectory;
        }

        public string WorkingDirectory { get; private set; }

        public void Save(int id, string message)
        {
            var path = this.GetFileName(id);
            File.WriteAllText(path, message);
        }

        public Maybe<string> Read(int id)
        {
            var path = this.GetFileName(id);
            if (!File.Exists(path))
                return new Maybe<string>();

            var message = File.ReadAllText(path);
            // Never want message to be Null - that is what the previous step is for
            return new Maybe<string>(message);
        }


        public string GetFileName(int id)
        {
            // This can never be null as int is a value type, and workingDirectory is a pre condition
            return Path.Combine(this.WorkingDirectory, id + ".txt");
        }
    }
}
