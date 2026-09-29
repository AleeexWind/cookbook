Feature: Recipe ratings
    As an authorized user I want to set a rating for a recipe on the details page
    so that the average rating reflects user feedback

    Scenario: Authorized user sets a rating once
        Given There is a public recipe "Pasta Carbonara" with average rating 4.5 from existing ratings
        And I am logged in as "bob"
        And I have not rated the recipe "Pasta Carbonara"
        When I open the recipe "Pasta Carbonara" from the recipes page
        And I set the rating to 5 stars
        Then My rating for "Pasta Carbonara" should be 5 stars
        And I should not be able to change my rating
        And The average rating should be updated within 1-5 stars

    Scenario: Guest cannot set a rating
        Given There is a public recipe "Pasta Carbonara"
        And I am not logged in
        When I open the recipe "Pasta Carbonara" from the recipes page
        Then I should see the average rating within 1-5 stars
        And I should not be able to set a rating

    Scenario: Authorized user who already rated cannot edit
        Given There is a public recipe "Pasta Carbonara"
        And I am logged in as "bob"
        And I have already rated "Pasta Carbonara" with 4 stars
        When I open the recipe "Pasta Carbonara" from the recipes page
        Then I should see my rating as 4 stars
        And I should not be able to change my rating
