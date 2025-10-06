"use strict";

// -- Variables --
// Constants
const cellWidth = 32; 
const cellSpacing = 0;
const container = document.querySelector("#content");
const cells = [];
const finalLevel = 4; // Change to match the true final level

const keyboard = Object.freeze({
    R: 		    82,
	SPACE: 		32,
	LEFT: 		65, 
	UP: 		87, 
	RIGHT: 		68, 
	DOWN: 		83,
    ONE:        49,
    TWO:        50,
    THREE:      51,
    FOUR:       52
});

const worldTile = Object.freeze({
	FLOOR: 		    0,
	WALL: 		    1,
	END: 		    2,
    BREAKABLEWALL:  3,
    EXPLOSION:      4
});

// Non Constants
let currentLevelNumber = 1;
let currentGameWorld = undefined; 
let renderedGameWorld = undefined;
let reachedEnd = false;
let completeAudioPlayed = false; // Need to limit the complete audio from playing multiple times
let effectAudio = undefined;

// Player Object
const player = Object.seal({
	x:-1,
	y:-1,
	element: undefined,
	moveRight(){this.x++;},
	moveDown(){this.y++;},
	moveLeft(){this.x--;},
	moveUp(){this.y--;},
});

// -- Starting Code -- 
window.onload = ()=>{
    currentLevelNumber = 1; // Set current level to 1 here to reset progress after every reset
	levelSetup();
	drawGameObjects(currentGameWorld.gameObj);
    effectAudio = document.querySelectorAll(".effectAudio");
	setupEvents();
}


// -- Functions --
// Player set up
function playerSetup() {
    const node =  document.createElement("span");
	node.className = "gameObject";

    player.element = node.cloneNode(true);
	player.element.classList.add("player");
	container.appendChild(player.element);
}

// Created a function to simplify the three times this code is repeated, since none of this code require a function specific parameter
function levelSetup() {
    if (currentGameWorld == null) {
        currentGameWorld = new gameWorld(currentLevelNumber);
    }
    else {
        currentGameWorld.clearCurrent();
        currentGameWorld.rebuildCurrent(currentLevelNumber);
    }
    renderedGameWorld = JSON.parse(JSON.stringify(currentGameWorld.map));
	let numCols = currentGameWorld.map[0].length;
	let numRows = currentGameWorld.map.length;
	createGridElements(numRows,numCols);
	drawGrid(currentGameWorld.map);
	loadLevel(currentLevelNumber);
    displayLevel();
}

// Creates a grid the size of the map
function createGridElements(numRows,numCols){
    container.innerHTML = " ";
    playerSetup();
	const span = document.createElement('span');
	span.className = 'cell';
	for (let row=0;row<numRows;row++){
	cells.push([]);
		for (let col=0;col<numCols;col++){
			let cell = span.cloneNode();
			cell.style.left = `${col * (cellWidth+cellSpacing)}px`;
			cell.style.top = `${row * (cellWidth+cellSpacing)}px`;
			container.appendChild(cell);
			cells[row][col] = cell;
		}
	}
}

// Loads the player and all of the object in the specified level
function loadLevel(levelNum = 1){
	currentGameWorld.gameObj = []; // clear out the old array
    currentGameWorld.rebuildObjects(levelNum);
	const node =  document.createElement("span");
	node.className = "gameObject";

    player.x = currentGameWorld.startPos[0];
	player.y = currentGameWorld.startPos[1];
	
	// Loop through this level's objects
	for (let obj of currentGameWorld.gameObj){
		const clone = { ...obj}; 		
		clone.element = node.cloneNode(true); 		
		clone.element.classList.add(obj.className); 			
		container.appendChild(clone.element);		
	}
}

// Initial draw request
function drawGrid(array){
	const numCols = array[0].length;
	const numRows = array.length;
	for (let row=0;row<numRows;row++){
		for (let col=0;col<numCols;col++){
			const tile = array[row][col];
            const element = cells[row][col];

            // Clear previous classes
            element.className = 'cell';

            // Potentially change from using switchw
			switch(tile) {
    			case worldTile.FLOOR:
        		element.classList.add("floor")
        		break;
        		
        		case worldTile.WALL:
        		element.classList.add("wall");
        		break;
        		
        		case worldTile.END:
        		element.classList.add("end");
        		break;

                case worldTile.BREAKABLEWALL:
                element.classList.add("breakablewall");
                break;

                case worldTile.EXPLOSION:
                element.classList.add("explosion");
                break;
			}
		}
	}
}

// Redraw changed tiles
function redrawTile(array, row, col){
	const tile = array[row][col];
    const defaultTile = currentGameWorld.map[row][col];

    if (tile !== defaultTile) {
    const element = cells[row][col];

    // Clear previous classes
    element.className = 'cell';

    // Potentially change from using switch
	switch(tile) {
    	case worldTile.FLOOR:
        element.classList.add("floor")
        break;
        		
    	case worldTile.WALL:
        element.classList.add("wall");
        break;
        		
        case worldTile.END:
        element.classList.add("end");
        break;

        case worldTile.BREAKABLEWALL:
        element.classList.add("breakablewall");
        break;

        case worldTile.EXPLOSION:
        element.classList.add("explosion");

        // I had to use chatGPT to find out a method of finding the length of gifs, 
        // which allows me to set the length of the timeout
        setTimeout(() => {
            array[row][col] = worldTile.FLOOR;
            redrawTile(array, row, col)
        }, 660); 

        break;
		}
	}
}

function drawGameObjects(array){
	// player
	player.element.style.left = `${player.x * (cellWidth + cellSpacing)}px`;
	player.element.style.top = `${player.y * (cellWidth + cellSpacing)}px`;
	
	// game object
	for (let gameObject of array){
		gameObject.element.style.left = `${gameObject.x * (cellWidth + cellSpacing)}px`;
		gameObject.element.style.top = `${gameObject.y * (cellWidth + cellSpacing)}px`;
	}
}

function playAudioByID(audioID) {
    effectAudio.forEach(element => {
        if (element.id == audioID) {
            const audio = new Audio(element.src);

            audio.volume = 0.03;
            audio.play();

            audio.addEventListener('ended', () => {
                audio.remove(); 
            });
        }
    });
}

// Each direction has to use its own directional movement, so I can't write it as one method
// The if check makes sure there is a wall to push off of, and the while case continuously moves the character while checking for a wall before the movement.
// Breakable walls are codesd so that they can be pushed off of as many times as the player wants, but the player can't land on them.
function movePlayer(e) {

    let nextX;
	let nextY;
    let nextTile;
    switch (e.keyCode) {
        case keyboard.UP:
            nextX = player.x;
		    nextY = player.y - 1;
            nextTile = renderedGameWorld[nextY][nextX];
            if ((renderedGameWorld[player.y+1][player.x] == worldTile.WALL || renderedGameWorld[player.y+1][player.x] == worldTile.BREAKABLEWALL) && renderedGameWorld[player.y-1][player.x] !== worldTile.BREAKABLEWALL) {
                while (nextTile != worldTile.WALL) {
                    if (nextTile == worldTile.BREAKABLEWALL) {
                        renderedGameWorld[nextY][nextX] = worldTile.EXPLOSION;
                        redrawTile(renderedGameWorld, nextY, nextX)
                        playAudioByID("wallBreak");
                    }
                    player.moveUp();
                    nextX = player.x;
		            nextY = player.y - 1;
                    nextTile = renderedGameWorld[nextY][nextX];
                };
            };

            break;

        case keyboard.DOWN:
            nextX = player.x;
		    nextY = player.y + 1;
            nextTile = renderedGameWorld[nextY][nextX];
            if ((renderedGameWorld[player.y-1][player.x] == worldTile.WALL || renderedGameWorld[player.y-1][player.x] == worldTile.BREAKABLEWALL) && renderedGameWorld[player.y+1][player.x] !== worldTile.BREAKABLEWALL) {
                while (nextTile != worldTile.WALL) {
                    if (nextTile == worldTile.BREAKABLEWALL) {
                        renderedGameWorld[nextY][nextX] = worldTile.EXPLOSION;
                        redrawTile(renderedGameWorld, nextY, nextX)
                        playAudioByID("wallBreak");
                    }
                    player.moveDown();
                    nextX = player.x;
		            nextY = player.y + 1;
                    nextTile = renderedGameWorld[nextY][nextX];
                };
            };
            break;
        
        case keyboard.LEFT:
            nextX = player.x - 1;
		    nextY = player.y;
            nextTile = renderedGameWorld[nextY][nextX];
            if ((renderedGameWorld[player.y][player.x+1] == worldTile.WALL || renderedGameWorld[player.y][player.x+1] == worldTile.BREAKABLEWALL) && renderedGameWorld[player.y][player.x-1] !== worldTile.BREAKABLEWALL) {
                while (nextTile != worldTile.WALL) {
                    if (nextTile == worldTile.BREAKABLEWALL) {
                        renderedGameWorld[nextY][nextX] = worldTile.EXPLOSION;
                        redrawTile(renderedGameWorld, nextY, nextX)
                        playAudioByID("wallBreak");
                    }
                    player.moveLeft();
                    nextX = player.x - 1;
		            nextY = player.y;
                    nextTile = renderedGameWorld[nextY][nextX];
                };
            };
            break;

        case keyboard.RIGHT:
            nextX = player.x + 1;
		    nextY = player.y;
            nextTile = renderedGameWorld[nextY][nextX];
            if ((renderedGameWorld[player.y][player.x-1] == worldTile.WALL || renderedGameWorld[player.y][player.x-1] == worldTile.BREAKABLEWALL) && renderedGameWorld[player.y][player.x+1] !== worldTile.BREAKABLEWALL) {
                while (nextTile != worldTile.WALL) {
                    if (nextTile == worldTile.BREAKABLEWALL) {
                        renderedGameWorld[nextY][nextX] = worldTile.EXPLOSION;
                        redrawTile(renderedGameWorld, nextY, nextX)
                        playAudioByID("wallBreak");
                    }
                    player.moveRight();
                    nextX = player.x + 1;
		            nextY = player.y;
                    nextTile = renderedGameWorld[nextY][nextX];
                };
            };
            break;
    }

    // Checking for the ending after movement forces the player to land on the end instead of just moving through it
    if(renderedGameWorld[player.y][player.x] == worldTile.END) {
        reachedEnd = true;
    }
    else {
        reachedEnd = false;
    };
}

function displayLevel(){
    document.getElementById("levelText").innerHTML = currentLevelNumber;
}

// -- Events -- 
function setupEvents(){
	window.onmouseup = (e) => {
		e.preventDefault();
	};
	
	window.onkeydown = (e)=>{

        // Seporate other inputs from moving to reduce actions
        if(e.keyCode == keyboard.R) {
            reachedEnd = false;
            levelSetup();
        }
        else if(e.keyCode == keyboard.ONE) {
            reachedEnd = false;
            currentLevelNumber = 1;
            levelSetup();
        }
        else if(e.keyCode == keyboard.TWO) {
            reachedEnd = false;
            currentLevelNumber = 2;
            levelSetup();
        }
        else if(e.keyCode == keyboard.THREE) {
            reachedEnd = false;
            currentLevelNumber = 3;
            levelSetup();
        }
        else if(e.keyCode == keyboard.FOUR) {
            reachedEnd = false;
            currentLevelNumber = 4;
            levelSetup();
        }
        else {
            movePlayer(e);
        }

        // Check if the player has reached the end after movement for instant transitions
        if (reachedEnd){
            if(currentLevelNumber == finalLevel) {
                document.getElementById("levelText").innerHTML = "Escaped!";
                if (!completeAudioPlayed) {
                    playAudioByID("gameComplete");
                    completeAudioPlayed = true;
                }
            }
            else{
                currentLevelNumber++;
                playAudioByID("levelChange");
                levelSetup();
                reachedEnd = false;
            }
		}
        else {
            completeAudioPlayed = false; // For the sake of my eardrums I must limit
        }

		drawGameObjects(currentGameWorld.gameObj);
	};
}

