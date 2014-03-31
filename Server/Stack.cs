using System;

public class Stack<T>
{

    class Node
    {
        public T data;
        public Node next;
    }

    private Node top;

    public Stack()
    {
        top = null;
    }

    public void push(T x)
    {
        Node t = new Node();
        t.data = x;
        t.next = top;
        top = t;
    }

    public T pop()
    {
        T x = top.data;
        top = top.next;
        return x;
    }

    public bool isMember(T x)
    {
        Node t = new Node();

        if (top == null)
        {
            return false;
        }

        t = top;

        while (t.data != null)
        {
            if (t.data.Equals(x))
            {
                return true;
            }
            t = top.next;
        }
        return false;
    }

    public bool isEmpty()
    {
        return top == null;
    }


    public void display()
    {
        Node t = top;
        Console.Write("\nStack contents are:  ");

        while (t != null)
        {
            Console.Write("{0} ", t.data);
            t = t.next;
        }
        Console.Write("\n");
    }
}

// class StackTest
// {
// 
//     public static void Main()
//     {
//         Stack<string> s = new Stack<string>();
//         Console.Write("Stack is created\n");
// 
//         s.push("?"); s.push("for"); s.push("looking"); s.push("you're"); s.push("me"); s.push("it"); s.push("is"); s.push("Hello,");
//         s.display();
// 
//         Console.WriteLine(s.isMember("it"));
// 
//         string i = s.pop();
//         Console.Write("\nJust popped {0}", i);
//         s.display();
//     }
// }

