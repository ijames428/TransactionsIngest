# Transactions Ingest

Built with Angular 22.2.3 and .NET 10.0.402.

Heads up that I'll probably pretty loose with the README's content.  I don't know that I've ever written one, so I'm going to use this as a one-stop shop for setup, use, my responses to items on the coding exercise, and my general thoughts and notes.

## TLDR

<b>Guessed 5.5 hours and it took me 18.25 hours.

Only AI usage was Google's AI search responses and followup questions there.

Lack of unit testing is, in my opinion, far and away the weakest part of this submission.</b>

## Setup

Download the zip file from Github and unzip it.

To start a local development server, run the Setup.bat file.  It'll fire up the frontend and the backend.

Open a web browser to http://localhost:4200/, which should show the frontend.

## Use

At the top, you can type in specific information and select the Source System and send a single transaction.  The page will update automatically with the state of the database, so you can change things in there and see how the database tables update.

If you hit Run Imports, a series of hard-coded values will be imported into the database.  Half of these are pulled from the coding exercise.  The other half are ones I added simply for author testing.

If you hit Reset Databases, all the data will be erased from the Transactions and History tables.  I added this purely to be able to test faster.  It's not meant to be a user-facing feature.

Below that is the frontend representations of the Transactions and History tables.

## Code Exercise Items

### Time

I estimated 5.5 hours initially, with this explanation from my raw notes below:

```plain
Time estimation: 5.5 hours - Setting up the project took about 15 minutes since I had to update packages.  I still need to set up SQLite, which I think will take another 15 minutes, because that's not something I do regularly.  I think the actual code won't take me that long, maybe 4 hours. I'll tack on an hour for potential unforeseen issues and call it 5.5 hours of work.
```

I was WAY off in regard to the entire project, which rang in at an estimated 18 hours and 15 minutes.  Even if I'd doubled my estimate, as per some Software Engineering wisdom, I would've still been off by a third.  The 5.5 hours ended up being closer to the actual "setup" time before I was able to really dig into the implementation of the core features.

It's definitely possible that I spent too much time on things to make development smoother, like the Reset Databases button.  Though I've found those types of features to pay off in the long run in professional settings since it's likely you'll be doing something like that frequently throughout development.

### Design decisions and assumptions

I can't really think of many assumptions I made.  There were some things I didn't fully understand at first, but for the most part, it was very straight forward across the board.

I considered adding some more database tables for things like the possible Source Systems and Location Codes, but I didn't want to overengineer it for the test.  If I were working on a real project, I'd strongly consider adding things like so we could move away from any hardcoded selection choices.  I have this written down in my raw notes below, but I also would likely get rid of the Location Code field.  In the current spec, it seems redundant.

My webdev skills are pretty rusty, but I'm happy with the code as it is, though I have no doubt a seasoned webdev could find many things to improve.

## What I feel I did wrong and would change

By far, in my opinion, the thing I did wrong was in regard to testing.  I simply haven't worked in environments with test cases since so much of what I did required you to run the video game and play to see if something happened, which isn't conducive to test cases and suites.  Those aren't things I would ignore when working on a project that would be in use.  I'd spend a lot more time on that.

I don't think things are well organized in terms of Access Specifiers.  Because I wasn't terribly familiar with what the default accessibility was for things in typescript, and because all the code for the lion's share of the functionality is in Program.cs, I was very loose about that.

I don't believe it was mentioned in the exercise details, but there's no sanitization of inputs on the frontend.  It doesn't seem like "real" version of this would even have an input like that, but it's something I chose to ignore because it didn't seem worthwhile for this test.

## AI Usage Disclosure

I only used Google's AI search results, which I found to be incredibly helpful for the more esoteric setup.  I did send further queries quite often to that, which was also helpful.  A lot of my questions to it were "I know I can do this, but I don't know how to make that happen, can you show me an example."  This was especially helpful during the work on getting revokes processed.  Trying to make sure I was only considering transactions from the last 24 hours would have been incredibly difficult, or hacky, without Google.

There was no use of project-side AI, though.

## Raw notes I took throughout development

Transactions Ingest

Got the latest versions of ng and node and nvm and all that stuff.
Had ng build the project and opened it in VS Code.
Installed the Angular Language Service.

-Miscellaneous
Time estimation: 5.5 hours - Setting up the project took about 15 minutes since I had to update packages.  I still need to set up SQLite, which I think will take another 15 minutes, because that's not something I do regularly.  I think the actual code won't take me that long, maybe 4 hours. I'll tack on an hour for potential unforeseen issues and call it 5.5 hours of work.

1:30pm-
I don't know that I'd set up SourceSystem as a string.  I think I'd make a table of all the sources with a unique id, the location ("Store Uptown", "Store Downtown", "Online"), and the locationCode ("UPTOWN", "DOWNTOWN", "ONLINE").  I think having both of these is superfluous though.  Maybe something will come up as I get further into the project that explains why those two columns are needed and can't be consolidated.
Seems like I've got the database set up in a nascent state, so now I'm going to get that information to display on the frontend.
Leaving to pick up my daughter from school.
-3:00pm

10:40am-
Wow, I'm learning a lot about how to set this stuff up.  Not my wheelhouse, but I'm making good progress.
Lunch.
-12:00pm

Few days later.

12:56pm-
I seem to have all the necessary parts in place now.  I have the frontend trying to call a test api function I added.  I'll continue working on this later.
-2:23pm

Saturday
1:30pm-5:45pm
8:30pm-10:30pm

Sunday
10:00am-5:30pm
9:00pm-sometime