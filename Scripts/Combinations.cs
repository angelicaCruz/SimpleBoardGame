using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataMesh.AR.Utility;

public class Combinations : MonoBehaviour
{
    //list of possibile winning combinations
    public List<string> c1= new List<string>
    {
        "Quad9",
        "Quad8",
        "Quad7"
    };

    public List<string> c2 = new List<string>
    {
        "Quad9",
        "Quad2",
        "Quad3"
    };

    public List<string> c3 = new List<string>
    {
        "Quad9",
        "Quad1",
        "Quad5"
    };

    public List<string> c4 = new List<string>
    {
        "Quad2",
        "Quad1",
        "Quad6"
    };

    public List<string> c5 = new List<string>
    {
        "Quad3",
        "Quad4",
        "Quad5"
    };

    public List<string> c6 = new List<string>
    {
        "Quad3",
        "Quad1",
        "Quad7"
    };

    public List<string> c7 = new List<string>
    {
        "Quad8",
        "Quad1",
        "Quad4"
    };

    public List<string> c8= new List<string>
    {
        "Quad7",
        "Quad5",
        "Quad6"
    };

    public List<string> getc1()
    {
        return c1;
    }

    public List<string> getc2()
    {
        return c2;
    }

    public List<string> getc3()
    {
        return c3;
    }

    public List<string> getc4()
    {
        return c4;
    }

    public List<string> getc5()
    {
        return c5;
    }

    public List<string> getc6()
    {
        return c6;
    }

    public List<string> getc7()
    {
        return c7;
    }
}


