## Setup secrets

Advent of C# provides the ability to automatically download your personalized missing input file for the specified problem.
To enable this, you need to specify your secret cookie session. Don't worry, it's only used for the purposes of communicating with the event's servers.

### Disclaimer

**No private data is collected by using any of this project's features, including this one.**

### Steps

First of all, you need a class [like the one in the template](https://github.com/AlFasGD/AdventOfCSharp.Template/blob/master/AdventOfCSharp.Template/MyCookies.cs), that will contain your cookie secret strings. Note that the `SecretsContainer` attribute is necessary. Do not create more than one such classes.

**IMPORTANT**: Do not forget to include your cookie file in .gitignore, if you're open-sourcing your solutions using Git!

Then, you need to find your actual cookie. The most obvious and painless way is to use a network tracking mechanism. In Google Chrome, you can easily do this:

- Connect to your account on the event site
- Open Inspect Element (<kbd>Ctrl</kbd> + <kbd>Shift</kbd> + <kbd>I</kbd>)
- Navigate to any page on the site (preferably anything that would be personalized, like the problems, the input, the settings, etc.)
- In the loaded document, find the request header name `cookie`
- Get the `_ga` and `session` values and put them in the respective properties in your class

### Example screenshot

![Chrome cookie discovery instructions](images/cookie_retrieval.png)

Note that secrets are (partially) hidden.
