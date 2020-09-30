using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class main : MonoBehaviour
{
    public Text TxtScreen;
    public Text TxtScreenResult;
    public Text TxtConverterFrom;
    public Text TxtConverterTo;
    public Text rateconversiontext;
    public GameObject ConverterObject;
    public GameObject nointernetobj;
    public Dropdown listfrom;
    public Dropdown listto;
    private List<string> currencies = new List<string> { "AUD", "BGN", "BRL", "CAD", "CHF", "CNY", "CZK", "DKK", "EUR", "GBP", "HKD", "HRK", "HUF", "IDR", "ILS", "INR", "JPY", "KRW", "MXN", "MYR", "NOK", "NZD", "PHP", "PLN", "RON", "RUB", "SEK", "SGD", "THB", "TRY", "USD", "ZAR" };
    private string equation = "";
    private bool readyToClear = false;
    private float result;

    void Start()
    {
        listfrom.AddOptions(currencies);
        listto.AddOptions(currencies);
    }
    
    void Update()
    {
        //ELEGXOS GIA SYNDESH STO DIADIKTYO (APAITEITAI GIA TH METATROPH NOMISMATON)
        if (ConverterObject.active)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                nointernetobj.SetActive(true);
            }
            else
            {
                nointernetobj.SetActive(false);
            }
        }
    }
    //PROSTHIKI ARITHMON KAI PRAKSEON STHN EKSISOSH
    public void AddCharToEquation(string character)
    {
        if (readyToClear == true && (!(character == "+" || character == "-" || character == "*" || character == "/")))
        {
            equation = "";
            TxtScreen.text = "";
            TxtScreenResult.text = "0";
            readyToClear = false;
        }
        else if (readyToClear == true && (character == "+" || character == "-" || character == "*" || character == "/"))
        {
            equation = TxtScreenResult.text;
            TxtScreen.text = "" + equation;
            readyToClear = false;
        }
        if (character == "d")
        {
            equation = equation.Substring(0, equation.Length - 1);
            TxtScreen.text = equation;
        }
        else if (character == "c")
        {
            equation = "";
            TxtScreen.text = "";
            TxtScreenResult.text = "0";
        }
        else if (character == "=")
        {
            TxtScreen.text += "=";
            readyToClear = true;
        }
        else if (character == "+" || character == "-" || character == "*" || character == "/")
        {
            bool enterif = true;
            if (equation.Length > 1)
            {
                if (equation[equation.Length - 1].ToString() == "+" || equation[equation.Length - 1].ToString() == "-" || equation[equation.Length - 1].ToString() == "*" || equation[equation.Length - 1].ToString() == "/")
                {
                    enterif = false;
                }
            }
            if ((!(equation.Length == 0/* && character != "-"*/)) && enterif == true)
            {
                ShowResult();
                equation += character;
                TxtScreen.text = equation;
            }
        }
        else
        {
            equation += character;
            TxtScreen.text = equation;
        }
    }
    //BGAZEI TO APOTELESMA THS PRAKSHS
    public void ShowResult()
    {
        result = evaluteQuestion(equation);
        TxtScreenResult.text = "" + result;
    }
    // ANOIGMA KAI KLEISIMO METATROPEA
    public void OpenCloseConverter()
    {
        readyToClear = true;
        ShowResult();
        ConverterObject.SetActive(!ConverterObject.active);
        TxtConverterFrom.text = TxtScreenResult.text;
    }
    // SYNDESH ME "Fixer.io" KAI APOTELESMA METATROPHS
    public void ShowConversionResult()
    {
        try
        {
            HttpWebRequest rq = (HttpWebRequest)WebRequest.Create("http://data.fixer.io/api/latest?access_key=19e5d6c8d170d9a249e53b55deb9057c&base=" + currencies[listfrom.value - 1] + "&symbols=" + currencies[listto.value - 1]);
            HttpWebResponse resp = (HttpWebResponse)rq.GetResponse();
            Stream s = resp.GetResponseStream();
            StreamReader TextReader = new StreamReader(s);
            string shtml = TextReader.ReadToEnd();
            string[] newstring = shtml.Split(':');
            string rate = newstring[newstring.Length - 1];
            rate = rate.Substring(0, rate.Length - 2);
            float postconversion = Convert.ToSingle(TxtConverterFrom.text) * Convert.ToSingle(rate, CultureInfo.InvariantCulture);
            TxtConverterTo.text = "" + postconversion;
            rateconversiontext.text = "Rate: " + rate;

            Debug.Log(Convert.ToSingle(rate, CultureInfo.InvariantCulture));
        }
        catch(Exception e)
        {
            TxtConverterTo.text = "Unavailable conversion :/";
            rateconversiontext.text = "Rate: -.--";
        }
    }
    // YPOLOGISMOS EKSISOSHS - EPISTREFEI TO APOTELESMA
    public float evaluteQuestion(string question)
    {
        float result = 0;
        int i = 0;
        string[] splitString = question.Split('+', '-', '*', '/');
        char[] splitfull = question.ToCharArray();

        List<string> ast = splitString.OfType<string>().ToList();   //Arithmoi
        List<string> bst = new List<string>();     //Prosima

        int l = 0;
        for (int k = l; k < splitfull.Length; k++)
        {
            if (splitfull[k] == '-' || splitfull[k] == '+' || splitfull[k] == '*' || splitfull[k] == '/')
            {
                bst.Add(splitfull[k].ToString());
            }
        }
        while (bst.Contains("*") || bst.Contains("/"))
        {
            if (bst[i].ToString() == "*")
            {
                ast[i] = (System.Convert.ToSingle(ast[i]) * System.Convert.ToSingle(ast[i + 1])).ToString();
                ast.RemoveAt(i + 1);
                bst.RemoveAt(i);
            }
            else if (bst[i].ToString() == "/")
            {
                ast[i] = (System.Convert.ToSingle(ast[i]) / (float)System.Convert.ToSingle(ast[i + 1])).ToString();
                ast.RemoveAt(i + 1);
                bst.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
        i = 0;
        while (bst.Count != 0)
        {
            if (bst[i].ToString() == "+")
            {
                ast[i] = (System.Convert.ToSingle(ast[i]) + System.Convert.ToSingle(ast[i + 1])).ToString();
                ast.RemoveAt(i + 1);
                bst.RemoveAt(i);
            }
            else if (bst[i].ToString() == "-")
            {
                ast[i] = (System.Convert.ToSingle(ast[i]) - System.Convert.ToSingle(ast[i + 1])).ToString();
                ast.RemoveAt(i + 1);
                bst.RemoveAt(i);
            }
        }
        try
        {
            result = System.Convert.ToSingle(ast[0]);
        }
        catch (Exception e)
        {

        }
        return result;
    }
}
