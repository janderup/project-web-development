export class AuctionImagePreview extends HTMLElement {

    constructor() {
        super();
        this.applyEventListeners();
    }

    applyEventListeners() {
        // Voeg een event listener toe voor het klikken op de delete-knop
        this.addEventListener('click', (event) => {
            if (event.target.classList.contains('delete-btn')) {
                this.delete(); // Roep de delete methode aan als de delete-knop wordt geklikt
            }
        });
    }

    connectedCallback() {
        const src = this.getAttribute('src');
        let html = '';

        html += `<div class='actions'><button class='delete-btn'>Verwijderen</button></div>`;
        html += `<img src='${src}'>`;

        this.innerHTML = html;
    }

    delete() {
        // Verwijder dit element uit de DOM
        this.parentNode.removeChild(this);
    }
}

// Definieer de nieuwe custom element
customElements.define('auction-image-preview', AuctionImagePreview);