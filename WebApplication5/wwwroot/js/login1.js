const inputs = document.querySelectorAll(".input");

$("#startNav").remove();
function addcl(){
	let parent = this.parentNode.parentNode;
	parent.classList.add("focus");
}

function remcl(){
	let parent = this.parentNode.parentNode;
	if(this.value == ""){
		parent.classList.remove("focus");
	}
}


inputs.forEach(input => {
	input.addEventListener("focus", addcl);
	input.addEventListener("blur", remcl);
	if (input.value.length > 0) {
		input.parentNode.parentNode.classList.add("focus");
    }
});