using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.TashClient.Helpers {
    public class Wait {
        public static void Until(Func<bool> condition, TimeSpan timeSpan) {
            var miliSeconds = timeSpan.Milliseconds + 1000 * timeSpan.TotalSeconds;
            var internalMiliSeconds = (int)Math.Ceiling(1 + miliSeconds / 20);
            do {
                if (condition()) { return; }

                Thread.Sleep(internalMiliSeconds); // Do not use await Task.Delay here
                miliSeconds = miliSeconds - internalMiliSeconds;
            } while (miliSeconds >= 0);

        }

        public static async Task UntilAsync(Func<Task<bool>> condition, TimeSpan timeSpan) {
            var miliSeconds = timeSpan.Milliseconds + 1000 * timeSpan.TotalSeconds;
            var internalMiliSeconds = (int)Math.Ceiling(1 + miliSeconds / 20);
            do {
                if (await condition()) { return; }

                Thread.Sleep(internalMiliSeconds); // Do not use await Task.Delay here
                miliSeconds = miliSeconds - internalMiliSeconds;
            } while (miliSeconds >= 0);

        }
    }
}
