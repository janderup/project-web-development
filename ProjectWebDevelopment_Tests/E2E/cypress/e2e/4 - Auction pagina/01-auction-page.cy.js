const url = 'https://localhost:7284/Auctions/Details/1';

context('Window', () => {
    beforeEach(() => {
        cy.visit(url)
    })

    it('Shows a title', () => {
        cy.get('[data-cy="page-title"]').should('be.visible')
    })

    it('Shows a description', () => {
        cy.get('[data-cy="auction-description"]').should('be.visible')
    })

    it('Shows a list of images', () => {
        cy.get('[data-cy="auction-images"]').should('be.visible');
    })

    it('Shows a countdown', () => {
        cy.get('[data-cy="auction-countdown"]').should('be.visible')
    })

    it('Shows a list of bids', () => {
        cy.get('[data-cy="list-of-bids"]').should('be.visible')
    })
})