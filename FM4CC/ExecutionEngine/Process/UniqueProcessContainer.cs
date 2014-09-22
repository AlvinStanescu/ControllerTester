using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.ExecutionEngine.Process
{
    public class UniqueProcessContainer<T> : IDisposable, ProcessContainer<T> where T : ApplicationProcess, new()
    {
        public static volatile string Path;
        private static volatile UniqueProcessContainer<T> instance;
        private static object syncRoot = new Object();
        private T process;
        private LinkedList<IObserver<bool>> observers;

        private IObserver<bool> processOwner;
        

        /// <summary>
        /// Constructor for the ProcesContainer
        /// - creates the process
        /// </summary>
        private UniqueProcessContainer()
        {
            process = new T();
            process.StartProcess(Path);
            observers = new LinkedList<IObserver<bool>>();
        }

        /// <summary>
        /// Destructor for the ProcesContainer
        /// - closes the process
        /// </summary>
        ~UniqueProcessContainer()
        {
        }

        /// <summary>
        /// Multi-threading safe singleton getter
        /// </summary>
        public static UniqueProcessContainer<T> Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new UniqueProcessContainer<T>();
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Requests ownership of the process instance for an object
        /// </summary>
        /// <param name="newOwner">The object which should own the MatLab instance</param>
        /// <returns>True if ownership has been achieved, false if the process has to be notified of ownership</returns>
        public bool RequestOwnership(IObserver<bool> newOwner)
        {
            if (processOwner == null)
            {
                lock (syncRoot)
                {
                    if (processOwner == null)
                    {
                        processOwner = newOwner;
                        return true;
                    }
                    else
                    {
                        observers.AddFirst(newOwner);
                        return false;
                    }
                }
            }
            else
            {
                lock (syncRoot)
                {
                    observers.AddFirst(newOwner);
                }
                return false;
            }
        }

        /// <summary>
        /// Gets a weak reference to the process. The reference will be invalidated 
        /// </summary>
        /// <param name="owner">owner object</param>
        /// <returns>A weak reference to the process that will be invalidated upon relinquishing ownership</returns>
        public T GetProcess(IObserver<bool> owner)
        {
            if (processOwner != null && processOwner == owner)
            {
                return process;
            }
            else
            {
                throw new InvalidOperationException("Attempted to get the process while not owning it");
            }
        }

        /// <summary>
        /// Relinquishes ownership of the process, invalidating the reference.
        /// </summary>
        /// <param name="currentOwner">owner object</param>
        public void RelinquishOwnership(IObserver<bool> currentOwner)
        {
            if (process == null || currentOwner == null)
            {
                throw new ArgumentException("Process cannot be null.");
            }
            else if (currentOwner != processOwner)
            {
                throw new InvalidOperationException("Cannot relinquish ownership if not the owner.");
            }
            else
            {
                processOwner = null;

                // if a process is waiting, set it as the new owner  
                if (observers.Count > 0)
                {
                    lock (syncRoot)
                    {
                        if (observers.Count > 0)
                        {
                            IObserver<bool> newOwner = observers.Last();
                            observers.RemoveLast();

                            processOwner = newOwner;

                            newOwner.OnNext(true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cancels the waiting process, removing the object from the waiting list for the process
        /// </summary>
        /// <param name="waitingObject">Object waiting to get control of the process</param>
        public void CancelWaiting(IObserver<bool> waitingObject)
        {
            lock (syncRoot)
            {
                observers.Remove(waitingObject);
            }
        }

        /// <summary>
        /// Method to request a restart of the controlled process
        /// </summary>
        /// <param name="owner"></param>
        public void RequestProcessRestart(IObserver<bool> owner)
        {
            if (owner != null && processOwner == owner)
            {
                process.EndProcess();
                process.StartProcess(Path);
            }
        }

        public void Dispose()
        {
            process.EndProcess();
        }

        public void Kill()
        {
            process.Kill();
        }

    }
}
