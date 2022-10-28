using System;
using System.Threading;

namespace CSharp14
{
    class Player
    {
        //static 변수
        // => 프로그램 시작 시 할당된다.
        //객체가 할당된다고 개별적으로 또 만들어지지 않는다.
        public static object lockObj = new object();

        //맴버 변수
        //=>객체가 할당(인스턴스)될때 개별적으로 할당된다.
        // ststic과 다르게 객체 별로 다른 값을 가진다.
        Thread thread;
        int index = 0;
        bool isCounting;
        public Player(int index)
        {
            this.index = index;
        }
        public void StartCount()
        {
            Thread thread = new Thread(Counting);
            thread.Start();
        }
        public void StopCount()
        {
            isCounting = false;
        }
        private void Counting()
        {
            int count = 0;
            isCounting = true;
            while (isCounting)
            {
                //lock
                //lockObj를 사용하여 잠금을 걸면 멀티 쓰레드 환경에서 
                // 다른 lockObj를 사용하는 lock문을 대기시킬수 있다.
                // A가 Lock을 걸고있으면 b는 진입못하다가 a가 1을돌면 lock이 풀리고 b도 같이돈다 

                lock (lockObj)
                {
                    Console.CursorLeft = 0;
                    Console.CursorTop = index;
                    Console.Write($"Player{index} : {count++}");

                    Console.CursorLeft = 0;
                    Console.CursorTop = 3;
                }
                Thread.Sleep(1000);
            }
        }
    }
    internal class Program
    {
        private delegate int NumberEvent(int a, int b);
        private delegate string ConcatEvent(params string[] strs);
        private delegate void TestEvent(int a, float b, string c);

        static void Main(string[] args)
        {
            if (false)
            {
                //이벤트
                // 어떠한 일이 발생하였다

                Timer timer = new Timer();     
                Player player1 = new Player(0);
                Player player2 = new Player(1);

                timer.onEndTimer += player1.StartCount;     //플레이어1의 StopCount함수를 타이머 이벤트에 등록.
                timer.onEndTimer += player2.StartCount;     //플레이어2의 StopCount함수를 타이머 이벤트에 등록.

                //event 델리게이트는
                //외부에서는 오로지 추가, 제거만 가능하다.
                //외부에서 호출하거나 어떠한 값을 대입하는 기능을 제한한다.

                //timer.onEndTimer();                 //델리게이트를 호출한다(who? Main이)
                //timer.onEndTimer = null;            //델리게이트를 지워버린다(누가? Main이)

                player1.StartCount();
                player2.StartCount();

                timer.StartTimer(10);

            }
            if (false)
            {

                NumberEvent onSum = Sum;


                Console.WriteLine(onSum(10, 20));
                //익명 메소드(=이름 없는 함수)
                onSum = delegate (int a, int b)
                 {
                     return a + b;
                 };

                //람다식
                // => 익명 메소드를 만들기 위한 표현식.
                onSum = (a, b) => a + b;


                //문 형식의 람다식 
                ConcatEvent onConcat = null;
                onConcat = (strs) =>
                {
                    string result = string.Empty;
                    for (int i = 0; i < strs.Length; i++)
                        result += strs[i];
                    return result;
                };
                Console.WriteLine(onConcat("ABCD", "EFG", "HIJ", "K"));

                TestEvent onTest = null;
                onTest = Print;
                //기본적인 익명메소드로 표현하는 방법
                onTest = delegate (int num, float height, string str)
                {

                };
                //익명 메소드를 람다식으로 포현(문형식)
                onTest = (num, height, str) =>
                {

                };

                for (int i = 0; i < 10; i++)
                {

                    Console.WriteLine(i);
                }
                //약식으로 표현(1줄로 포현하는 방법)
                onTest = (num, height, str) => Console.WriteLine($"num:{num},height{height},str{str}");
                onTest(10, 30.5f, "Test");
            }
            //Func,Action을 이용하는 델리게이트
            //Func<Result T> 반환형이 있는 댈리게이트를 표현하는 방법.
            //Action: 반환형이 없는 델리게이트를 표현하는 방법.

            //1. 반환형이 없고 int를 하나 받는 델리게이트 변수가 필요하다
            // = delegate void SomeEvent(int num);
            Action<int> onPrint = (number) => Console.WriteLine($"number:{number}");
            onPrint(10000);

            //2. int를 반환하고 int를 2개 받는 델리게이트 변수가 필요하다.
            //=delegate string SomeEvent(int a, int b);
            //마지막에 오는게 반환형
            Func<int, int, string> onSum2 = (a, b) => a + b.ToString();
            Console.WriteLine(onSum2(300, 400));

            //3. 반환형이 없고 string 2개를  받는 델리게이트
            //반환형이 없기에 WriteLine
            Action<string, string> onPrint1 = (str1, str2) => Console.WriteLine(str1 + str2);

            //4.반환형이  int이고 float를 하나 받는 델리게이트
            //형변환 사용해야함
            Func<float, int> onPrint2 = (at) => (int)at;
            
            //5.반환형이 string 이고 string배열를 받는 델리게이트
            Func<string[], string> onPrint3 = (strs) => {
                string result = string.Empty;
                foreach (string str in strs)
                    result += str;
                return result;
            };

            //6. 반환형이 없고 매개변수도 없는 델리게이트
            Action onPrint4 = () => Console.WriteLine("없음");
            
            //7. 반환형이 int이고 매개변수가 없는 델리게이트
            Func<int> onPrint5 = () => 20;

            //델리게이트가 null인 상태에서 호출을 하면 NullReference 예외가 발생한다.
            Action onCallback = null;

            //해결방법 1. IF처리
            if (onCallback != null)
                onCallback();

            //2 Nullable 체크 처리(Invoke)
            onCallback?.Invoke();   //델리게이트가 Null이 아니면 Invoke(호출) 해라. (null이면 무시한다)

            Person p = null;
            p?.talk();
            //??: p가 null이 아니라면 age를 null이라면 0을 사용한다

            Console.WriteLine($"사람의 나이 :{p?.age?? 0}");

            int money = 0;      //0이라는 값이 있는것이다 (사람의 기준으로 돈이 없다 가 아니다)
            int? gold = null;   //값형식의 자료형 뒤에 붙이면 null형식을 지원하겠다(nullable 타입의 값 형식)
            Console.WriteLine($"골드 : { (gold == null ? "골드가 없다.":gold)}");

            //예외는 요청을 받은 '객체' 가 스스로 판단해서 throw해주는 것이다.
            //null이라면 그throw해줄 대상이 없기에 try문에서는 처리가 불가능하다.
          /*  try
             {
                 onCallback();
             }
             catch(Exception ex)
             {

             }*/
        }
        

        class Person
        {
            public int age;
            public Person(int age)
            {
                this.age = age;
            }
            public void talk()
            {
                Console.WriteLine($"{age}살이 말한다!");
            }
        }
        //3개의 매게변수출력 람다식 
        static void Print(int a, float height, string str)
        {

        }   

        static int Sum(int a, int b)
        {
            return a + b;
        }
    }
}

