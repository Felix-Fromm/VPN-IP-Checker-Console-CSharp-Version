using System.Diagnostics;
using System;
using System.Net;
using System.Net.Http.Json;


//--------------------------------------
//----------- Settings Code ------------
//--------------------------------------

// Setting some Strings with the Path and the File name of the Settings file.
string sSettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)+ "\\IP-VPN-Checker"; //LocalAppData -> /IP-VPN-Checker
string sConfigName = "config.ff";

if(!Directory.Exists(sSettingsPath)) //Checks if the Settings Folder exists
{ 
    Directory.CreateDirectory(sSettingsPath); //Creates it when it doesnt exist.
}

if (!File.Exists(sSettingsPath + "\\"+sConfigName)) //Asks inverted if the File Exists ( it really asks if it is not exisiting)
{
    Console.WriteLine("You can create a Account and get a API-Key from 'VPNAPI.io' | If you write 'visit' then the site will open for you!");
    BackAfterWebsiteVisit:
    Console.WriteLine("Insert API-Key:");
    string input = Console.ReadLine();
    if(input.ToLower() == "visit")
    {
        Process.Start("explorer", "https://vpnapi.io/signup");
        goto BackAfterWebsiteVisit;
    } else
    {
        try
        {
            File.WriteAllText(sSettingsPath + "\\" + sConfigName, input); // Saves your Key into a File and saves it inside the Settings Folder.
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message); //DEBUG Error Print
            Console.Error.WriteLine(ex.StackTrace); //DEBUG Error Print
        }
    }
} 
else
{
    //This Triggers when it found a Settings file and asks you if you want to rewrite it.
    WrongTextReset:
    Console.WriteLine("Do you want to edit your key? Yes/No (empty = No)");
    string input = string.Empty;
    input = Console.ReadLine();
    if(input == "yes")
    {
        NewCodeReset: // Reset for empty Key.
        Console.WriteLine("Type your new key code now:");
        string newKey = Console.ReadLine(); //Reads your Input and saves it in newKey
        if(newKey == null || newKey == string.Empty)
        {
            Console.WriteLine("Pls enter a valid Code.");
            goto NewCodeReset; // When the input is null or empty set him back to reenter his Key
        }
        File.WriteAllText(sSettingsPath + "\\" + sConfigName, newKey); 
    } else if (input == "no" || input == string.Empty) { } //End Editmode when no or "" is detected. (Ending with just no code in it)
    else //When no predefined word matches
    {
        Console.WriteLine("Pls enter with one of the answers.");
        goto WrongTextReset; //Back to start of Key Reset if typed answer is not a defined one.
    }
}

string sKey = File.ReadAllText(sSettingsPath + "\\" + sConfigName); //Trys to Read Settings file and saves it into the var sKey
while (sKey == null)
{
    sKey = File.ReadAllText(sSettingsPath + "\\" + sConfigName); //If it Failes then it trys agian till it worked.
}

//-------------------------------
//--------- Main Code -----------
//-------------------------------
BeginOfProcess: //Loops back here wenn one run is finished

Console.WriteLine("Please Type a IP Adress (Example: 8.8.8.8):"); 
string sIpToLookUp = Console.ReadLine(); //Saves IP as String for URL Build.

string sAPIURL = "https://vpnapi.io/api/"+sIpToLookUp+"?key=" + sKey; //Building the API key with the LookUpIP and your Key.

HttpClient client = new HttpClient(); //Creating a HTTPClient for a web request.
HttpResponseMessage clientresponse = await client.GetAsync(sAPIURL); // Calls the Website and saves its response in clientresponse
clientresponse.EnsureSuccessStatusCode(); //Checks if everything is ok
string sResult = await clientresponse.Content.ReadAsStringAsync(); //awaits to get the content of the Page as a String.

Console.WriteLine(sResult); //Print Output from sResult
goto BeginOfProcess; //Loop to Start for new Search
