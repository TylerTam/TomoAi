<h1>Tomo AI</h1>
<i>Last updated 2024-01-19</i>
<h2>About</h2>
Tomo Ai is a project I started that is inspired by the game Tomodachi Life, when I wanted to create a project with AI Text generation systems. <br/>
This project is meant to work with Kobold AI, and uses web requests to send prompts, and recieve the generated texts. 

<h2>Cloning</h2>
Before cloning, you should know that Kobold AI requires a lot of VRAM. If you don't have atleast 8gb of free VRAM, you can't run this program. You'll also want more than 8gb, if you want better, more coherent text generation.<br/>
<br>Additionally, if you only have 8gb of VRAM, you won't be able to reliably run both the AI generator, and the unity project, and you'll need a seperate device to run just the server, and another to run just the unity project.</br>

This project doesn't actually contain the AI text generation AI. You will need to install Kobold AI, locally to run this, or connect to an open server that is running kobold AI. 
<br/><br/>
Here is a <a href = "https://www.reddit.com/r/PygmalionAI/comments/10dj8gl/i_found_out_how_to_run_it_localy_with_kobold_ai/">guide on installing Kobold AI</a><br>
<br/><br/>
If the link is broken, I have a local copy of their instructions saved, so just let me know.
<br/><br/>

If you are running a local version of Kobold AI, there may be a change you need to make, if you are running Kobold, and Unity on seperate devices.
<h3>Remote-Play.bat</h3>
Go into the kobold AI file location, and find the remote-play.bat file. Duplicate this file and name it 'Remote-Play-LocalServer.bat'. Open the renamed file in a text editor (notepad works fine) and adjust the line that says <br/>

```
play --remote %*
```
so that it says

```
play --host %*
```

<h2>Connecting to Kobold</h2>
Before opening unity, you'll need to find the IP address of the device you are running Kobold on. <br/>
On windows, you can do this by opening command prompt, and typing in <b>ipconfig</b> and pressing enter. The one you need is your ipv4 address. <br/>

Nowopen the unity project, or the build, and there will be a settings button in the top right corner of the starting screen. Open it, and you'll see the input field to paste the IP. After this, you should be connected.

<h2>Emotional Analysis - NRCLex</h2>
This project also has an option for emotion analysis on both the user's input text and the generated text from the AI. This is used to adjust the AI's mood depending on what the player says. <br/>
I used NRCLex for this, and hile there may be better options for emotion analysis, my server was already tied up with Kobold AI, and for research purposes NRCLex worked fine.<br>


<h3>Setting Up NRCLex</h3>
If you want to set up the sentiment analysis, you'll need to install NRCLex through python. Here is the <a href = "https://pypi.org/project/NRCLex/">link on pypi</a> <br/>

To create the server to run this, you can simply paste this code into a python script, and run it. 
```
from flask import Flask

from nrclex import NRCLex

app = Flask(__name__)

@app.route('/')
def checkConnect():
    return 'connected'

@app.route('/getemotion/<text>')
def get_emotion(text):
    emotion = NRCLex(text)
    return emotion.top_emotions
    

    
if __name__ == '__main__':
    app.run(debug=True, port=5001)
```
<h3>Alternate emotional analysis</h3>
If you want to use another method for emotional analysis, it's pretty simple to change it in this project. You'll just need to change the emotional analysis url when running the game.<br/>
That said, you'll need to make sure that the output is identical to NRCLex's output. That, or you adjust how this project reads the new output.
