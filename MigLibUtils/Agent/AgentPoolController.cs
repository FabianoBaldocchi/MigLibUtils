using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MigLibUtils.Agent
{
    public abstract class AgentPoolController<TData>
        where TData : AgentData

    {
        public enum NEXT_STEP_FEED { CONTINUE = 0, STOP = 1, WAIT = 2, CONTINUE_CHECK_UNICITY = 3 }
        public enum TIPO_LOG { ERRO = 0, ALERTA = 1, TRACE = 2 }

        internal ConcurrentQueue<TData> Queue = new ConcurrentQueue<TData>();

        ConcurrentDictionary<string, Agent<TData>> DicAgents = new ConcurrentDictionary<string, Agent<TData>>();

        internal Agent<TData>.AGENT_STATUS Status = Agent<TData>.AGENT_STATUS.WAITING;

        internal Thread FeedThread;

        internal int DEFAULT_TIMER_FEED_MS = 0;

        internal int QUEUE_MIN_SIZE_TO_CALL_FEED = int.MaxValue;

        internal int DEFAULT_TIMER_FEED_WAIT_MS = 1000 * 60;
        internal int DEFAULT_TIMER_FEED_SUSPENDED_MS = 1000;

        internal int TimerFeedMs = 0;

        internal SemaphoreSlim ThrottingSemaphore;
        internal int? ThrottingIntervalMillisecond;

        public void Init(int qty, int? timerFeedMs = null, int? throttingIntervalMillisecond = null, int? throttingQtyLimit = null)
        {
            //inicializa as threads no status do pool
            Qty = qty;

            AdjustFeed(timerFeedMs);

            if (throttingQtyLimit != null && (int)throttingQtyLimit > 0)
            {
                //inicializa os semaforos
                ThrottingSemaphore = new SemaphoreSlim((int)throttingQtyLimit, (int)throttingQtyLimit);
            }

            ThrottingIntervalMillisecond = throttingIntervalMillisecond;

        }

        int Qty
        {
            get
            {
                return DicAgents.Count;
            }
            set
            {
                var delta = value - DicAgents.Count;
                if (delta < 0)
                {
                    for (int i = DicAgents.Count - 1; i > delta; i--)
                    {
                        //remove agentes
                        var key = i.ToString();
                        if (DicAgents.TryRemove(key, out Agent<TData> ag))
                            ag.Stop();
                    }
                }
                else if (delta > 0)
                {
                    for (int i = DicAgents.Count; i < delta; i++)
                    {
                        var c = CreateItem();
                        c.Set(this, i.ToString());
                        if (c != null)
                        {
                            DicAgents[i.ToString()] = c;
                        }
                    }
                }
            }
        }

        public abstract Agent<TData> CreateItem();

        public void SetThreads(int qty, int? timerFeedMs = null)
        {
            Qty = qty;

            AdjustFeed(timerFeedMs);
        }

        public int QueueCount()
        {
            return Queue.Count;
        }

        public int AgentCount()
        {
            return DicAgents.Count;
        }

        internal void Finished(Agent<TData> agent)
        {
            DicAgents.TryRemove(agent.Id, out Agent<TData> oag);
        }

        public virtual void Start()
        {
            Status = Agent<TData>.AGENT_STATUS.ACTIVE;

            DicAgents.Values.ToList().ForEach(a => a.Start());

            StartFeed();
        }

        public void Enqueue(TData elem)
        {
            Queue.Enqueue(elem);
        }


        public virtual void Stop(bool force = false, int waitingtime = 500)
        {
            Status = Agent<TData>.AGENT_STATUS.INACTIVE;

            StopFeed();

            DicAgents.Values.ToList().ForEach(a => a.Stop(force, waitingtime));

            if (!force)
            {
                while (DicAgents.Count > 0)
                {
                    Log("Stop.Wait", "Qtd Agents:" + DicAgents.Count, TIPO_LOG.TRACE);
                    Thread.Sleep(1000);
                }
            }
        }

        internal void StartFeed()
        {
            if (FeedThread == null && TimerFeedMs > 0)
            {
                FeedThread = new Thread(new ThreadStart(FuncThreadFeed));
                FeedThread.Start();
            }
        }

        private void FuncThreadFeed()
        {
            while (Status != Agent<TData>.AGENT_STATUS.INACTIVE)
            {
                while (Status == Agent<TData>.AGENT_STATUS.WAITING)
                {
                    Thread.Sleep(DEFAULT_TIMER_FEED_SUSPENDED_MS);
                }

                if (Status == Agent<TData>.AGENT_STATUS.INACTIVE)
                {
                    break;
                }

                if (Queue.Count < QUEUE_MIN_SIZE_TO_CALL_FEED)
                {
                    try
                    {
                        var ret = Feed(out string error, out TData[] FeedData);

                        if (ret == NEXT_STEP_FEED.STOP)
                            break;

                        else if (ret == NEXT_STEP_FEED.WAIT)
                            Thread.Sleep(DEFAULT_TIMER_FEED_WAIT_MS);

                        else
                        {
                            if (FeedData?.Length > 0)
                                FeedData.ToList().ForEach(fd =>
                                {
                                    if (ret == NEXT_STEP_FEED.CONTINUE)
                                        Queue.Enqueue(fd);

                                    else if (ret == NEXT_STEP_FEED.CONTINUE_CHECK_UNICITY)
                                    {
                                        var cont = FeedQueueContains(fd, out TData equal);
                                        if (!cont)
                                            Queue.Enqueue(fd);
                                        else
                                            fd.ProcessDiscarded(equal);
                                    }
                                });
                        }

                    }
                    catch (Exception ex)
                    {
                        Log("Feed.Exec", ex.ToString(), TIPO_LOG.ERRO);
                    }
                }

                if (TimerFeedMs > 0)
                    Thread.Sleep(TimerFeedMs);
            }
        }

        internal void StopFeed()
        {
            if (FeedThread != null)
            {
                try
                {
                    FeedThread.Abort();
                }
                catch (Exception ex)
                {
                    Log("StopFeed.ThreadAbort", ex.ToString(), TIPO_LOG.ERRO);
                }

                FeedThread = null;

            }
        }

        internal void AdjustFeed(int? timerFeed)
        {
            if (timerFeed == null)
            {
                timerFeed = DEFAULT_TIMER_FEED_MS;
            }

            if ((int)timerFeed == 0)
            {
                StopFeed();
                return;
            }

            TimerFeedMs = (int)timerFeed;

        }

        public virtual bool FeedQueueContains(TData data, out TData equal)
        {
            equal = null;
            var qu = Queue.ToList();

            foreach (var q in qu)
                if (q.IsEquals(data))
                {
                    equal = q;
                    return true;
                }

            return false;
        }

        protected List<TData> GetQueueCopy()
        {
            return Queue.ToList();
        }

        public int FreeSlots()
        {
            if (ThrottingSemaphore == null)
                return -1;

            return ThrottingSemaphore.CurrentCount;
        }


        public abstract void Log(string local, string Msg, TIPO_LOG tipo);

        protected abstract NEXT_STEP_FEED Feed(out string error, out TData[] NewContent);


    }
}
