using System.Threading.Tasks;
using UnityEngine;

namespace zFramework.Ex
{
    public static class TaskExtension
    {
        public static Task<int> WhenAny(params Task[] tasks)
        {
            var tcs = new TaskCompletionSource<int>();
            try
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    var cached = i;
                    var task = tasks[i];
                    task.ContinueWith(t =>
                    {
                        var result = t.IsCanceled ? -1 : cached;
                        tcs.TrySetResult(result);
                    });
                }
            }
            catch (System.Exception e) 
            {
                tcs.TrySetResult(-1);
            }
            return tcs.Task;
        }
    }
}
