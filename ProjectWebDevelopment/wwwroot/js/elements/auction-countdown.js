export class AuctionCountdown extends HTMLElement {

    constructor() {
        super();
        this.bindEvents();
    }

    bindEvents() {
        setInterval(this.updateCountdown.bind(this), 1000);
    }

    updateCountdown() {
        const endDate = Date.parse(this.getAttribute('end-date'));
        const now = Date.now();
        const difference = endDate - now;

        if (difference < 0) {
            this.innerHTML = 'Verlopen';
            return;
        }

        const hours = Math.floor(difference / (1000 * 3600));
        const minutes = Math.floor((difference % (1000 * 3600)) / (1000 * 60));

        this.innerHTML = `${hours}u ${minutes}m`;
    }

    connectedCallback() {
        this.updateCountdown();
    }
}
customElements.define('auction-countdown', AuctionCountdown);