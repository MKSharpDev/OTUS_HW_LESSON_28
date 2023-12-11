using System.Collections.Generic;
using System.Diagnostics;



//Сумма массива обычным способом  small = 451325
//Время затраченное на операцию обычным способом с массивом small = 00:00:00.0015367
//Сумма массива при помощи Task small = 451325
//Время затраченное на операц при помощи Paralle с массивом small = 00:00:00.1401488
//Сумма массива при помощи Parallel small = 451325
//Время затраченное на операц при помощи Paralle с массивом small = 00:00:00.0229043
//Сумма массива при помощи PLINQ small = 451325
//Время затраченное на операцию при помощи PLINQ с массивом small = 00:00:00.0185045

//Сумма массива обычным способом  medium = 4506575
//Время затраченное на операцию обычным способом с массивом medium = 00:00:00.0067145
//Сумма массива при помощи Task medium = 4506575
//Время затраченное на операц при помощи Paralle с массивом medium = 00:00:01.1380815
//Сумма массива при помощи Parallel medium = 4506575
//Время затраченное на операц при помощи Paralle с массивом medium = 00:00:00.0283790
//Сумма массива при помощи PLINQ medium = 4506575
//Время затраченное на операцию при помощи PLINQ с массивом medium = 00:00:00.0016800

//Сумма массива обычным способом  large = 45009805
//Время затраченное на операцию обычным способом с массивом large = 00:00:00.0748375
//Сумма массива при помощи Task large = 45009805
//Время затраченное на операц при помощи Paralle с массивом large = 00:00:07.8830629
//Сумма массива при помощи Parallel large = 45009805
//Время затраченное на операц при помощи Paralle с массивом large = 00:00:00.2414294
//Сумма массива при помощи PLINQ large = 45009805
//Время затраченное на операцию при помощи PLINQ с массивом large = 00:00:00.0118639




int[] intS = AddRandomIntsInArray(100000);
int[] intM = AddRandomIntsInArray(1000000);
int[] intL = AddRandomIntsInArray(10000000);
Sum(intS, "small");
Sum(intM, "medium");
Sum(intL, "large");

Console.ReadKey();

async void Sum(int[] arr, string arrName)
{
    Stopwatch sw = Stopwatch.StartNew();
    sw.Start();
    int rezultOrdinary = arr.Sum();
    sw.Stop();
    TimeSpan timeRezOrdinary = sw.Elapsed;


    sw.Restart();

    int rezultThreadParallel = 0;
    Parallel.ForEach(arr, x => Interlocked.Add(ref rezultThreadParallel, x));

    sw.Stop();
    TimeSpan timeRezThreadParallel = sw.Elapsed;


    sw.Restart();

    int rezultThread = await SumTask(arr);

    sw.Stop();
    TimeSpan timeRezThread = sw.Elapsed;

    sw.Restart();
    int rezultPLINQ = arr.AsParallel().Sum();
    sw.Stop();
    TimeSpan timeRezPLINQ = sw.Elapsed;


    Console.WriteLine($"Сумма массива обычным способом  {arrName} = {rezultOrdinary}");
    Console.WriteLine($"Время затраченное на операцию обычным способом с массивом {arrName} = {timeRezOrdinary}");
    Console.WriteLine($"Сумма массива при помощи Task {arrName} = {rezultThread}");
    Console.WriteLine($"Время затраченное на операц при помощи Paralle с массивом {arrName} = {timeRezThread}");
    Console.WriteLine($"Сумма массива при помощи Parallel {arrName} = {rezultThreadParallel}");
    Console.WriteLine($"Время затраченное на операц при помощи Paralle с массивом {arrName} = {timeRezThreadParallel}");
    Console.WriteLine($"Сумма массива при помощи PLINQ {arrName} = {rezultPLINQ}");
    Console.WriteLine($"Время затраченное на операцию при помощи PLINQ с массивом {arrName} = {timeRezPLINQ}");
    Console.WriteLine();
}


int[] AddRandomIntsInArray(int leght)
{
    int[] resultArr = new int[leght];
    Random rand = new Random();

    for (int i = 0; i < resultArr.Length; i++)
    {
        resultArr[i] = rand.Next(0, 10);
    }
    return resultArr;
}

async Task<int> SumTask(int[] arr)
{

    int cores = Environment.ProcessorCount;
    int result = 0;

    List<int> innArr = arr.ToList();
    List<List<int>> innArrSplitList = Split(innArr, cores);

    var parallelTask = new List<Task<int>>();
    foreach (var item in innArrSplitList)
    {
        parallelTask.Add(Task.Factory.StartNew(() =>
        {
            int toAdd = item.Sum();
            return toAdd;
        }));
    }

    await Task.WhenAll(parallelTask);
    result = parallelTask.Sum(t => t.Result);
    return result;

}

static List<List<T>> Split<T>(IList<T> source, int count)
{
    return source
        .Select((x, i) => new { Index = i, Value = x })
        .GroupBy(x => x.Index / count)
        .Select(x => x.Select(v => v.Value).ToList())
        .ToList();
}