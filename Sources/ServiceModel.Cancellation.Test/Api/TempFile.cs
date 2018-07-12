using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Api
{
    [DebuggerDisplay("{Location}")]
    internal sealed class TempFile : IDisposable
    {
        public TempFile()
        {
            Location = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }

        public string Location { get; }

        public static TempFile FromResources(string resourceName)
        {
            resourceName.IsNotNullAndNotEmpty(nameof(resourceName));

            var owner = new StackTrace().GetFrame(1).GetMethod().DeclaringType;
            Assert.IsNotNull(owner);

            var result = new TempFile();

            using (var source = owner.Assembly.GetManifestResourceStream(owner, resourceName))
            {
                Assert.IsNotNull(source, resourceName);
                using (var destination = new FileStream(result.Location, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    source.CopyTo(destination);
                }
            }

            return result;
        }

        public void Dispose()
        {
            if (File.Exists(Location))
            {
                File.Delete(Location);
            }
        }
    }
}
