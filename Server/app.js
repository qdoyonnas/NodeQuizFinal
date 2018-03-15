// REQUIRES
var http = require("http");
var path = require("path");
var socketIO = require("socket.io")(5000);
var mongoDb = require("mongodb");
var express = require("express");
var session = require("express-session");
var bodyParser = require("body-parser");
var cookieParser = require("cookie-parser");
var passport = require("passport");
var LocalStrategy = require("passport-local").Strategy;
var logger = require("morgan");

// APP VALUES
var port = 3000;
var app = express();

// Style
app.use(express.static(path.resolve(__dirname, "src/css")));

// App View Engine
app.set('views', path.resolve(__dirname, 'src/views'));
app.set('view engine', 'ejs');

// MongoDb
var dbName = "quizdata";
var url = "mongodb://qdoyonnas:abc123@ds053858.mlab.com:53858/quizdata";
var db;
mongoDb.connect(url, function(error, client)
{
	if( error ) { throw error; }
		
	console.log("Connected to MongoDB");
	db = client.db(dbName);
});

// App modules
app.use(logger("dev"));
app.use(bodyParser.urlencoded({extended:false}));
app.use(cookieParser());
app.use(session( {
	secret: "secretSession",
	resave: true,
	saveUninitialized: true
	}
));
app.use(passport.initialize());
app.use(passport.session());

// User Authentication Setup
passport.serializeUser(function(user, done) 
{
	done(null, user);
});
passport.deserializeUser(function(user, done) 
{
	done(null, user);
});
passport.use(new LocalStrategy(
	{
	usernameField: "",
	passwordField: ""
	},
	function(username, password, done) {
		db.collection("users").findOne({username:username}, function(error, result) 
		{
			if( result.password === password ) {
				var user = result;
				done(null, user);
			} else {
				done(null, false, {message:"Incorrect Password"});
			}
		});
	}
));

// Methods
function EnsureAuthenticated(request, response, next)
{
	if( request.isAuthenticated() ) {
		next();
	}
	else {
		response.redirect("/sign-in");
	}
}

// SOCKETIO
socketIO.on("connection", function(socket)
{
	console.log("A user connected");
	
	socket.on("sendData", function(data)
	{
	    console.log("User sending Data");
	    console.log(data["questions"]);
	});

	socket.on("loadData", function()
	{
	    console.log("User loading Data");

	    var data = db.collection("quizdata").find({}, function(error, results) 
		{
			if( error ) { throw error; }
			
			var arrayData;
			
			results.toArray().then(function(data) {
				arrayData = data;
			}).then(function() {
				console.log("Sending Data to user");
				console.log(arrayData);
				socket.emit("recieveData", arrayData)
			});
		});
	});

	socket.on("disconnect", function()
	{
		console.log("A user disconnected");
	});
});
console.log("Game server started on 5000");

// ROUTES
app.get("/", function(request, response)
{
	db.collection("quizdata").find().toArray(function(error, results)
	{
		if( error ) { throw error; }
		
		response.render("index", {entries: results});
	});
});

app.get("/sign-in", function(request, response)
{
	response.render("sign-in");
});
app.post("/sign-in", passport.authenticate("local",
{
	failureRedirect:"/sign-in"
	}), function(request, response) {
			response.redirect("/profile");
	}
);

app.get("/sign-up", function(request, response)
{
	response.render("sign-up");
});
app.post("/sign-up", function(request, response)
{
	db.collection("users").save(request.body, function(error, result) {
		if( error ) { throw error; }
		console.log("User Saved");
	});
	
	request.login(request.body, function() {
		response.redirect("/sign-in");
	});
});

app.get("/profile", EnsureAuthenticated, function(request, response)
{
	response.render("profile", {user: request.user});
});

app.get("/add-question", EnsureAuthenticated, function(request, response)
{
	response.render("add-question");
});
app.post("/add-question", function(request, response)
{
	if( !request.body.text
		|| !request.body.correct
		|| !request.body.wrong1 )
	{
		response.status(400).send("Riddles must have some text");
		return;
	}
	
	db.collection("quizdata").save(request.body, function(error, result) {
		if( error ) { throw error; }
		console.log("data saved");
		response.redirect("/");
	});
});

app.get("/delete-entry", function(request, response)
{
	if( request.query.id ) {
		var requestId = {_id: mongoClient.ObjectID(request.query.id)};
		db.collection(dbName).deleteOne(requestId, function(error, result) {
			if( error ) { throw error; }
			//console.log("data deleted: " + result);
			response.redirect("/");
		});
	} else {
		response.render("delete-entry");
	}
});

app.get("/logout", function(request, response)
{
	request.logout();
	response.redirect("/");
});

app.use(function(request, response)
{
	response.status(404).render("404");
});

// Start local Server
app.listen(port, function()
{
	console.log("Server listening on port " + port);
});
