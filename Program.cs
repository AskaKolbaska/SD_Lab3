using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Лабораторная работа №3");
        Console.WriteLine("090304-РПИб-о23");
        Console.WriteLine("Рыжкова Е. А.");

        // Ввод данных
        Console.WriteLine("Введите количество маршрутов:");
        int routeCount = int.Parse(Console.ReadLine());
        List<int[]> busRoutes = new List<int[]>();

        for (int i = 0; i < routeCount; i++)
        {
            Console.WriteLine($"Введите остановки для маршрута {i + 1}, разделенные пробелом:");
            int[] route = Console.ReadLine().Split().Select(int.Parse).ToArray();
            busRoutes.Add(route);
        }

        Console.WriteLine("Введите начальную и конечную остановки, разделенные пробелом:");
        int[] endpoints = Console.ReadLine().Split().Select(int.Parse).ToArray();
        int start = endpoints[0];
        int end = endpoints[1];

        // Реализация через массив
        Console.WriteLine("Реализация через массив:");
        MeasurePerformance(() => ArrayImplementation(busRoutes, start, end));

        // Реализация через связанный список
        Console.WriteLine("\nРеализация через связанный список:");
        MeasurePerformance(() => LinkedListImplementation(busRoutes, start, end));

        // Реализация через стандартные библиотеки
        Console.WriteLine("\nРеализация через стандартные библиотеки:");
        MeasurePerformance(() => StandardLibraryImplementation(busRoutes, start, end));

        Console.ReadLine();
    }

    static void MeasurePerformance(Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        Console.WriteLine($"Время выполнения: {stopwatch.Elapsed.TotalMilliseconds:F3} мс");
    }

    static void ArrayImplementation(List<int[]> busRoutes, int start, int end)
    {
        int n = busRoutes.Max(route => route.Max());
        int[,] graph = new int[n + 1, n + 1];
        List<int[]> routes = new List<int[]>();
        int[] path = new int[n];
        int pathIndex = 0;

        // Заполнение графа
        foreach (var route in busRoutes)
        {
            for (int i = 0; i < route.Length - 1; i++)
            {
                graph[route[i], route[i + 1]] = 1;
                graph[route[i + 1], route[i]] = 1;
            }
        }

        // Поиск всех путей между парой вершин
        DFS_Array(graph, start, end, path, ref pathIndex, routes);

        // Вывод найденных путей
        Console.WriteLine("Найденные пути (через массив):");
        foreach (var p in routes)
        {
            foreach (var stop in p)
            {
                Console.Write(stop + " ");
            }
            Console.WriteLine();
        }

        // Поиск наилучшего пути по условиям задачи
        var bestRoute = FindBestRoute(routes, busRoutes.ToArray());

        // Вывод лучшего пути и номера автобуса
        int bestBusNumber = GetBestBusNumber(bestRoute, busRoutes.ToArray());
        Console.WriteLine("Лучший путь:");
        foreach (var stop in bestRoute)
        {
            Console.Write(stop + " ");
        }
        Console.WriteLine($"\nНомер автобуса: {bestBusNumber}");
    }

    static void DFS_Array(int[,] graph, int current, int end, int[] path, ref int pathIndex, List<int[]> routes)
    {
        path[pathIndex] = current;
        pathIndex++;

        if (current == end)
        {
            int[] tempPath = new int[pathIndex];
            Array.Copy(path, tempPath, pathIndex);
            routes.Add(tempPath);
        }
        else
        {
            for (int i = 1; i < graph.GetLength(0); i++)
            {
                if (graph[current, i] == 1 && Array.IndexOf(path, i, 0, pathIndex) == -1)
                {
                    DFS_Array(graph, i, end, path, ref pathIndex, routes);
                }
            }
        }

        pathIndex--;
    }

    static void LinkedListImplementation(List<int[]> busRoutes, int start, int end)
    {
        int n = busRoutes.Max(route => route.Max());
        Dictionary<int, LinkedList<int>> graph = new Dictionary<int, LinkedList<int>>();
        List<List<int>> routes = new List<List<int>>();
        List<int> path = new List<int>();

        // Заполнение графа
        foreach (var route in busRoutes)
        {
            for (int i = 0; i < route.Length - 1; i++)
            {
                AddEdge_LinkedList(graph, route[i], route[i + 1]);
                AddEdge_LinkedList(graph, route[i + 1], route[i]);
            }
        }

        // Поиск всех путей между парой вершин
        DFS_LinkedList(graph, start, end, path, routes);

        // Вывод найденных путей
        Console.WriteLine("Найденные пути (через связанный список):");
        foreach (var p in routes)
        {
            foreach (var stop in p)
            {
                Console.Write(stop + " ");
            }
            Console.WriteLine();
        }

        // Поиск наилучшего пути по условиям задачи
        var bestRoute = FindBestRoute(routes.Select(p => p.ToArray()).ToList(), busRoutes.ToArray());

        // Вывод лучшего пути и номера автобуса
        int bestBusNumber = GetBestBusNumber(bestRoute, busRoutes.ToArray());
        Console.WriteLine("Лучший путь:");
        foreach (var stop in bestRoute)
        {
            Console.Write(stop + " ");
        }
        Console.WriteLine($"\nНомер автобуса: {bestBusNumber}");
    }

    static void AddEdge_LinkedList(Dictionary<int, LinkedList<int>> graph, int u, int v)
    {
        if (!graph.ContainsKey(u))
        {
            graph[u] = new LinkedList<int>();
        }
        graph[u].AddLast(v);
    }

    static void DFS_LinkedList(Dictionary<int, LinkedList<int>> graph, int current, int end, List<int> path, List<List<int>> routes)
    {
        path.Add(current);

        if (current == end)
        {
            routes.Add(new List<int>(path));
        }
        else
        {
            if (graph.ContainsKey(current))
            {
                foreach (var neighbor in graph[current])
                {
                    if (!path.Contains(neighbor))
                    {
                        DFS_LinkedList(graph, neighbor, end, path, routes);
                    }
                }
            }
        }

        path.RemoveAt(path.Count - 1);
    }

    static void StandardLibraryImplementation(List<int[]> busRoutes, int start, int end)
    {
        int n = busRoutes.Max(route => route.Max());
        Dictionary<int, List<int>> graph = new Dictionary<int, List<int>>();
        List<List<int>> routes = new List<List<int>>();
        List<int> path = new List<int>();

        // Заполнение графа
        foreach (var route in busRoutes)
        {
            for (int i = 0; i < route.Length - 1; i++)
            {
                AddEdge_StandardLibrary(graph, route[i], route[i + 1]);
                AddEdge_StandardLibrary(graph, route[i + 1], route[i]);
            }
        }

        // Поиск всех путей между парой вершин
        DFS_StandardLibrary(graph, start, end, path, routes);

        // Вывод найденных путей
        Console.WriteLine("Найденные пути (через стандартные библиотеки):");
        foreach (var p in routes)
        {
            foreach (var stop in p)
            {
                Console.Write(stop + " ");
            }
            Console.WriteLine();
        }

        // Поиск наилучшего пути по условиям задачи
        var bestRoute = FindBestRoute(routes.Select(p => p.ToArray()).ToList(), busRoutes.ToArray());

        // Вывод лучшего пути и номера автобуса
        int bestBusNumber = GetBestBusNumber(bestRoute, busRoutes.ToArray());
        Console.WriteLine("Лучший путь:");
        foreach (var stop in bestRoute)
        {
            Console.Write(stop + " ");
        }
        Console.WriteLine($"\nНомер автобуса: {bestBusNumber}");
    }

    static void AddEdge_StandardLibrary(Dictionary<int, List<int>> graph, int u, int v)
    {
        if (!graph.ContainsKey(u))
        {
            graph[u] = new List<int>();
        }
        graph[u].Add(v);
    }

    static void DFS_StandardLibrary(Dictionary<int, List<int>> graph, int current, int end, List<int> path, List<List<int>> routes)
    {
        path.Add(current);

        if (current == end)
        {
            routes.Add(new List<int>(path));
        }
        else
        {
            if (graph.ContainsKey(current))
            {
                foreach (var neighbor in graph[current])
                {
                    if (!path.Contains(neighbor))
                    {
                        DFS_StandardLibrary(graph, neighbor, end, path, routes);
                    }
                }
            }
        }

        path.RemoveAt(path.Count - 1);
    }

    static int[] FindBestRoute(List<int[]> routes, int[][] busRoutes)
    {
        int minWeight = int.MaxValue;
        int[] bestRoute = null;

        foreach (var route in routes)
        {
            int weight = CalculateRouteWeight(route, busRoutes);
            if (weight < minWeight)
            {
                minWeight = weight;
                bestRoute = route;
            }
        }

        return bestRoute;
    }

    static int CalculateRouteWeight(int[] route, int[][] busRoutes)
    {
        int weight = 0;
        int transfers = 0;

        for (int i = 0; i < route.Length - 1; i++)
        {
            weight++;
            if (!IsSameBus(route[i], route[i + 1], busRoutes))
            {
                transfers++;
            }
        }

        weight += transfers * 3;
        return weight;
    }

    static bool IsSameBus(int stop1, int stop2, int[][] busRoutes)
    {
        foreach (var route in busRoutes)
        {
            bool foundFirst = false;
            foreach (var stop in route)
            {
                if (stop == stop1) foundFirst = true;
                if (foundFirst && stop == stop2) return true;
            }
        }

        return false;
    }

    static int GetBestBusNumber(int[] route, int[][] busRoutes)
    {
        Dictionary<int, int> busUsage = new Dictionary<int, int>();

        for (int i = 0; i < route.Length - 1; i++)
        {
            for (int j = 0; j < busRoutes.Length; j++)
            {
                if (Array.IndexOf(busRoutes[j], route[i]) != -1 && Array.IndexOf(busRoutes[j], route[i + 1]) != -1)
                {
                    if (!busUsage.ContainsKey(j + 1))
                    {
                        busUsage[j + 1] = 0;
                    }
                    busUsage[j + 1]++;
                }
            }
        }

        return busUsage.OrderByDescending(x => x.Value).First().Key;
    }
}