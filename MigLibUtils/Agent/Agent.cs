using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MigLibUtils.Agent
{
    public abstract class Agent<TData>
        where TData : AgentData
    {
        public int SLEEP_WAITING_MS = 1000;

        public int SLEEP_QUEUE_AFTER_PROCESS = 0;
        public int SLEEP_QUEUE_EMPTY_CURRENT = 100;
        public int SLEEP_QUEUE_EMPTY_MS_MIN = 100;
        public int SLEEP_QUEUE_EMPTY_MS_MAX = 10000;
        public double SLEEP_QUEUE_EMPTY_MS_MULT = 1.5;


        public enum AGENT_STATUS { WAITING = 0, ACTIVE = 1, STOPPING = 2, INACTIVE = 99 };

        public enum RETURN_ACTION { REQUEUE = 0, DISCARD = 1 };

        AGENT_STATUS Status = AGENT_STATUS.WAITING;

        public AgentPoolController<TData> Parent { get; set; }

        Thread ThreadExec { get; set; }

        bool CanExecute = false;

        public string Id { get; set; }

        public void Set(AgentPoolController<TData> parent, string id)
        {
            Parent = parent;
            Id = id;
            CanExecute = true;
            Status = parent.Status;

            ThreadExec = new Thread(new ThreadStart(ExecThread));
            ThreadExec.Start();
        }

        internal void Stop(bool force = false, int waitingtimems = 1000)
        {
            Status = AGENT_STATUS.STOPPING;

            var limite = DateTime.Now.AddMilliseconds(waitingtimems);

            while (Status != AGENT_STATUS.INACTIVE && DateTime.Now < limite)
            {
                Thread.Sleep(10);
            }

            if (Status != AGENT_STATUS.INACTIVE && force)
            {
                Status = AGENT_STATUS.INACTIVE;
                CanExecute = false;
                try
                {
                    ThreadExec.Abort();
                }
                catch (Exception ex)
                {
                    Parent.Log("Agent.Stop.ThreadAbort", ex.ToString(), AgentPoolController<TData>.TIPO_LOG.ERRO);
                }
            }
        }

        internal void Start()
        {
            Status = AGENT_STATUS.ACTIVE;
        }

        protected void ExecThread()
        {
            while (CanExecute && Status != AGENT_STATUS.INACTIVE)
            {
                while (CanExecute && Status == AGENT_STATUS.WAITING)
                {
                    Thread.Sleep(SLEEP_WAITING_MS);
                }

                if (!CanExecute || Status == AGENT_STATUS.INACTIVE)
                {
                    break;
                }

                TData oNext = default;
                try
                {
                    oNext = GetNext();
                }
                catch (Exception ex)
                {
                    Parent.Log("Agent.GetNext", ex.ToString(), AgentPoolController<TData>.TIPO_LOG.ERRO);
                }

                if (oNext == null)
                {
                    if (Status == AGENT_STATUS.STOPPING)
                    {
                        break;
                    }

                    WaitQueueEmpty();
                }
                else
                {
                    HasQueueElement();

                    RETURN_ACTION s = RETURN_ACTION.REQUEUE;
                    Boolean executed = false;


                    //marca a data de fim se há throtting
                    DateTime? esperarAte = (Parent.ThrottingIntervalMillisecond == null || (int)Parent.ThrottingIntervalMillisecond == 0 ? 
                                                     null :
                                                     (DateTime?)DateTime.Now.AddMilliseconds((int)Parent.ThrottingIntervalMillisecond));

                    //trava uma transação se há throtting
                    if (Parent.ThrottingSemaphore != null)
                        Parent.ThrottingSemaphore.Wait();

                    try
                    {
                        s = ProcessAgentData(oNext);

                        executed = true;
                    }
                    catch (Exception ex)
                    {
                        Parent.Log("Agent.ProcessAgentData", ex.ToString(), AgentPoolController<TData>.TIPO_LOG.ERRO);
                    }

                    if (s == RETURN_ACTION.REQUEUE)
                    {
                        Parent.Queue.Enqueue(oNext);
                    }

                    //espera pelo tempo de throtting se necessario
                    if (esperarAte != null)
                    {
                        while ((DateTime)esperarAte > DateTime.Now)
                            Thread.Sleep(5);
                    }

                    //libera a thread para o próximo
                    if (Parent.ThrottingSemaphore != null)
                        Parent.ThrottingSemaphore.Release(1);


                    if (executed && SLEEP_QUEUE_AFTER_PROCESS > 0)
                        Thread.Sleep(SLEEP_QUEUE_AFTER_PROCESS);
                }
            }
            Status = AGENT_STATUS.INACTIVE;

            Parent.Finished(this);


        }

        private void HasQueueElement()
        {
            SLEEP_QUEUE_EMPTY_CURRENT = SLEEP_QUEUE_EMPTY_MS_MIN;
        }

        private void WaitQueueEmpty()
        {

            if (SLEEP_QUEUE_EMPTY_CURRENT == 0)
            {
                NoQueueElements();
                return;
            }

            SLEEP_QUEUE_EMPTY_CURRENT = (int)(SLEEP_QUEUE_EMPTY_CURRENT * SLEEP_QUEUE_EMPTY_MS_MULT);

            if (SLEEP_QUEUE_EMPTY_CURRENT == 0)
                SLEEP_QUEUE_EMPTY_CURRENT = SLEEP_QUEUE_EMPTY_MS_MIN;
            else if (SLEEP_QUEUE_EMPTY_CURRENT > SLEEP_QUEUE_EMPTY_MS_MAX)
                SLEEP_QUEUE_EMPTY_CURRENT = SLEEP_QUEUE_EMPTY_MS_MAX;

            if (SLEEP_QUEUE_EMPTY_CURRENT > 0)
            {
                Thread.Sleep(SLEEP_QUEUE_EMPTY_CURRENT + (int)(new Random().Next(10) / 10.0 * SLEEP_QUEUE_EMPTY_CURRENT));
            }

            NoQueueElements();

        }

        public virtual TData GetNext()
        {
            var b = Parent.Queue.TryDequeue(out TData agdata);
            if (!b)
                return default;

            return agdata;
        }

        public virtual RETURN_ACTION ProcessAgentData(TData oAD)
        {
            return RETURN_ACTION.DISCARD;
        }

        public virtual void NoQueueElements()
        {

        }

    }
}
