using System;
using System.Threading.Tasks;
using CopilotChat.WebApi.Models.Request;
using CopilotChat.WebApi.Models.Response;

namespace CopilotChat.WebApi.Services;

public class SingleMessageCompletionService
{
    private readonly int _randomNumber;

    //create object
    public SingleMessageCompletionService(){
        var random = new Random();
        _randomNumber = random.Next(1, 100);
    }

    public int GetNumber() => _randomNumber;

}

