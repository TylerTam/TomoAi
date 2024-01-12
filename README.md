<h1>Tomo AI</h1>
<i>Last updated 2024-01-06</i>
<h2>About</h2>
Tomo Ai is a project I started that is inspired by the game Tomodachi Life, when I wanted to create a project with AI Text generation systems. <br/>
This project is meant to work with Tavern AI & Kobold AI, and uses web requests to send prompts, and recieve the generated texts. 

<h2>Cloning</h2>
Before cloning, you should know that Kobold AI requires a lot of VRAM. If you don't have atleast 8gb of free VRAM, you can't run this program. You'll also want more than 8gb, if you want better, more coherent text generation.<br/>
<br>Additionally, if you only have 8gb of VRAM, you won't be able to reliably run both the AI generator, and the unity project, and you'll need a seperate device to run just the server, and another to run just the unity project.</br>

This project doesn't actually contain the AI text generation AI. You will need to install both Kobold AI, and Tavern AI locally to run this. Alternatively, if you have some node js knowledge, you can create a server that runs Kobold AI, since Tavern AI is mostly used for it's server (since I don't have much experience with creating a local server).
<br/><br/>
Here is a <a href = "https://www.reddit.com/r/PygmalionAI/comments/10dj8gl/i_found_out_how_to_run_it_localy_with_kobold_ai/">guide on installing Kobold AI</a><br>
Here is a <a href = "https://www.reddit.com/r/PygmalionAI/comments/10gv5hn/tutorial_for_tavernai_with_pygmalionai_locally/?rdt=60510">guide on installing Tavern AI</a><br>
<br/><br/>
If either link is broken, I have a local copy of their instructions saved, so just let me know.
<br/><br/>
After installing both, if you've installed Tavern AI, there are some changes you'll have to make in the server.js file, and config.conf file. <br/>
<h3>TavernAI - Config.conf</h3>
For any devices that you want to connect to tavern AI, you'll need to add their IP address to the whitelist array. You can also just set whitelist mode to false.<br/>
Next, change the csrf_token to false. Again, if you know how to work with csrf_tokens, you can alternatively address this in Unity, without changing it to false.<br/>
<h3>Tavern AI - Server.js</h3>
This file is where all the server calls are made. You'll basically need to make a unique server call for Unity here, but it's pretty simple.<br/>
Search for "webui', and you should find the "/generate_webui" function. You can copy this function, and create your own, or you can paste this blurb just above the function:
  
```
  app.post("/generate_unity", jsonParser, function (request, response_generate) {
    if (!request.body) return response_generate.sendStatus(400);
    //console.log(request.body.prompt);
    //const dataJson = json5.parse(request.body);

    console.log(request.body);


    let this_settings = {
        prompt: request.body.prompt,
        use_story: request.body.use_story,
        gui_settings: request.body.gui_settings,
        use_memory: request.body.use_memory,
        use_authors_note: request.body.use_authors_note,
        use_world_info: request.body.use_world_info,
        max_context_length: request.body.max_context_length,
        max_length: request.body.max_length,
        rep_pen: request.body.rep_pen,
        rep_pen_range: request.body.rep_pen_range,
        rep_pen_slope: request.body.rep_pen_slope,
        temperature: request.body.temperature,
        tfs: request.body.tfs,
        top_a: request.body.top_a,
        top_k: request.body.top_k,
        top_p: request.body.top_p,
        typical: request.body.typical,
        singleline: request.body.singleline,
    };

    
    var args = {
        data: this_settings,
        headers: { "Content-Type": "application/json" },
        requestConfig: {
            timeout: connectionTimeoutMS
        }
    };
    client.post(api_server + "/v1/generate", args, function (data, response) {
        if (response.statusCode == 200) {
            console.log(data);
            response_generate.send(data);
        }
        if (response.statusCode == 422) {
            console.log('Validation error');
            response_generate.send({ error: true, error_message: "Validation error" });
        }
        if (response.statusCode == 501 || response.statusCode == 503 || response.statusCode == 507) {
            console.log(data);
            if (data.detail && data.detail.msg) {
                response_generate.send({ error: true, error_message: data.detail.msg });
            } else {
                response_generate.send({ error: true, error_message: "Error. Status code: " + response.statusCode });
            }
        }
    }).on('error', function (err) {
        console.log(err);
        //console.log('something went wrong on the request', err.request.options);
        response_generate.send({ error: true, error_message: "Unspecified error while sending the request.\n" + err });
    });
    
});

```

<br/>
After inputting that, you can save your changes, and start both kobold and tavern AI. Remember to change the kobold model to something other than Read Only, after it opens. This project was built using Pygmalion 2b, but the other chat models should work as well.




