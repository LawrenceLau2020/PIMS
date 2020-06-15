PRINT 'Adding Workflows'

SET IDENTITY_INSERT dbo.[Workflows] ON

INSERT INTO dbo.[Workflows] (
    [Id]
    , [Name]
    , [Code]
    , [IsDisabled]
    , [Description]
    , [SortOrder]
) VALUES (
    1
    , 'Submit Surplus Property Process Project'
    , 'SUBMIT-DISPOSAL'
    , 0
    , 'Create a new Surplus Property Process Project to add properties to the Enhanced Referral Program or the Surplus Property List.'
    , 0
), (
    2
    , 'Access Surplus Property Process Project Request'
    , 'ACCESS-DISPOSAL'
    , 0
    , 'Assess a submitted Surplus Property Process Project to determine whether it will be approved or denied.'
    , 1
)

SET IDENTITY_INSERT dbo.[Workflows] OFF