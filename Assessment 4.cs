using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using CsvHelper;
using HtmlAgilityPack;


var token = "your_token";
var query = "Feedback";
var url = $"https://slack.com/api/search.messages?token={token}&query={query}";
var web = new HtmlWeb();
var doc = web.Load(url);

var messages = doc.DocumentNode.SelectNodes("//matches/message");

using (var writer = new StreamWriter("scraped_data.csv"))
using (var csv = new CsvWriter(writer))
{
    csv.WriteHeader<Message>();
    csv.NextRecord();
    foreach (var message in messages)
    {
        var text = message.SelectSingleNode("text").InnerText;
        if (Regex.IsMatch(text, query, RegexOptions.IgnoreCase))
        {
            var name = message.SelectSingleNode("username").InnerText;
            var date = DateTime.Parse(message.SelectSingleNode("ts").InnerText);
            var otherAttribute = message.SelectSingleNode("subtype").InnerText;
            csv.WriteRecord(new Message() { Name = name, Date = date, OtherAttribute = otherAttribute, Text = text });
            csv.NextRecord();
        }
    }
}

public class Message
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public string OtherAttribute { get; set; }
    public string Text { get; set; }
}
