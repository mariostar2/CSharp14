using System;
using System.Threading;

namespace CSharp14
{
    internal class Timer
    {
        public delegate void OnEndEvent();
        public event OnEndEvent onEndTimer;

        int time;
        public void StartTimer(int second)
        {
            time = second * 1000;                       //시간을 밀리초로 저장
            Thread thread = new Thread(OnTimer);        // 새로운 쓰레드를 생성
            thread.Start();                             // 쓰레드 시작
        }
        public void OnTimer()
        {
            Thread.Sleep(time);
            // Console.WriteLine("시간이 되었다:");
            onEndTimer();
        }
    }
}
