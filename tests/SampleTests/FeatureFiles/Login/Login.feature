Feature: User Login
  As a registered user
  I want to sign in with my username and password
  So that I can reach the product catalogue

# ── Happy Path ────────────────────────────────────────────────────

# Test Case Summary: Standard User signs in with valid credentials and lands on the product list
# Precondition: User has an active account with a known username and password
@SmokeAutomated @High @SBK-101 @ClaudeGeneratedTest @smokeBDD
Scenario: Standard User signs in with valid credentials and sees the product list
  Given the user is on the login page
  When the user enters a valid username and password
  Then the product list page should be displayed
  And the page heading should read "Products"

# ── Validation / Negative ────────────────────────────────────────

# Test Case Summary: Guest User submits credentials that match no account and is told they do not match
# Precondition: No account exists for the submitted username
@RegressionAutomated @Medium @SBK-101 @ClaudeGeneratedTest @smokeBDD
Scenario: Guest User enters unregistered credentials and sees a mismatch error
  Given the user is on the login page
  When the user enters a username that is not registered
  Then an error message should state "Username and password do not match"
  And the user should remain on the login page

# Test Case Summary: Guest User submits the form without a username and is asked for one
# Precondition: N/A
@RegressionAutomated @Medium @SBK-101 @ClaudeGeneratedTest @smokeBDD
Scenario: Guest User leaves the username blank and sees a required-field error
  Given the user is on the login page
  When the user submits a password only
  Then an error message should state "Username is required"
  And the user should remain on the login page

# Test Case Summary: Guest User submits the form without a password and is asked for one
# Precondition: N/A
@RegressionAutomated @Medium @SBK-101 @ClaudeGeneratedTest @smokeBDD
Scenario: Guest User leaves the password blank and sees a required-field error
  Given the user is on the login page
  When the user submits a username only
  Then an error message should state "Password is required"
  And the user should remain on the login page

# ── Business Rules & Restrictions ────────────────────────────────

# Test Case Summary: Locked User attempts to sign in and is blocked with a lockout message
# Precondition: The account has been locked by an administrator
@RegressionAutomated @Medium @SBK-101 @BusinessCase @ClaudeGeneratedTest @smokeBDD
Scenario: Locked User enters correct credentials and is refused with a lockout message
  Given the user is on the login page
  When the user enters the locked account's credentials
  Then an error message should state "this user has been locked out"
  And the user should remain on the login page

# ── Edge Cases ───────────────────────────────────────────────────

# Test Case Summary: Standard User enters the username in a different letter case and is not signed in
# Precondition: User has an active account with a known username and password
@RegressionAutomated @Low @SBK-101 @ClaudeGeneratedTest @smokeBDD
Scenario: Standard User enters the username in upper case and sees a mismatch error
  Given the user is on the login page
  When the user enters the valid username in upper case with the valid password
  Then an error message should state "Username and password do not match"

# Test Case Summary: Guest User submits several invalid combinations and each is rejected with the expected message
# Precondition: N/A
@RegressionAutomated @Low @SBK-101 @ClaudeGeneratedTest @smokeBDD
Scenario Outline: Guest User submits "<username>" / "<password>" and sees "<message>"
  Given the user is on the login page
  When the user enters "<username>" as the username and "<password>" as the password
  Then an error message should state "<message>"

  Examples:
    | username      | password     | message                            |
    |               | secret_sauce | Username is required               |
    | standard_user |              | Password is required               |
    | no_such_user  | wrong        | Username and password do not match |
