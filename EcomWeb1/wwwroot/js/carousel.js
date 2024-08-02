// JavaScript for infinite carousel
var carousel = document.getElementById('carousel');
var clone = carousel.cloneNode(true);
carousel.parentNode.replaceChild(clone, carousel);
clone.id = 'carousel2';

// Function to duplicate images in the carousel
function duplicateImages() {
    var images = clone.querySelectorAll('img');
    images.forEach(function (img) {
        var cloneImg = img.cloneNode(true);
        clone.appendChild(cloneImg);
    });
}

duplicateImages(); // Duplicate the images once
var speed = 50; // Adjust scrolling speed (lower values mean faster scrolling)

var carouselWidth = clone.offsetWidth;
var scrollWidth = clone.scrollWidth;
var offset = 0;

function scrollCarousel() {
    offset++;
    if (offset > scrollWidth - carouselWidth) {
        offset = 0;
    }
    clone.style.transform = 'translateX(' + -offset + 'px)';
}

var scrollInterval = setInterval(scrollCarousel, speed);

// Pause scrolling when mouse hovers over the carousel
clone.onmouseenter = function () {
    clearInterval(scrollInterval);
};

// Resume scrolling when mouse leaves the carousel
clone.onmouseleave = function () {
    scrollInterval = setInterval(scrollCarousel, speed);
};