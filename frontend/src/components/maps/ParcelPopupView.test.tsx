import React from 'react';
import renderer from 'react-test-renderer';
import { ParcelPopupView } from './ParcelPopupView';
import { Router } from 'react-router-dom';
import { createMemoryHistory } from 'history';
import { render } from '@testing-library/react';
import { useKeycloak } from '@react-keycloak/web';

const history = createMemoryHistory();

jest.mock('@react-keycloak/web');
(useKeycloak as jest.Mock).mockReturnValue({
  keycloak: {
    userInfo: {
      agencies: ['1'],
    },
    subject: 'test',
  },
});

const mockParcel = (agencyId: string) => {
  var parcel = {
    id: 1,
    pid: '1',
    pin: '1',
    latitude: '1',
    longitude: '1',
    statusId: '1',
    propertyStatus: 'Test Property Status',
    municipality: 'Test Municipality',
    projectNumber: 'Test-Project-Number',
    classification: 'Test Classification',
    description: 'Test Description',
    landArea: '100',
    classificationId: 1,
    zoning: '',
    agencyId: agencyId,
    isSensitive: false,
    landLegalDescription: 'Test Land Legal Description',
    address: '1234 Test Addr',
    evaluations: [],
    buildings: [],
    fiscals: [
      {
        fiscalYear: 2020,
        key: 'Key',
        value: 'Value',
      },
    ],
    zoningPotential: '',
  };
  return parcel;
};

it('renders correctly', () => {
  const tree = renderer
    .create(
      <Router history={history}>
        <ParcelPopupView parcel={mockParcel('1')} />
      </Router>,
    )
    .toJSON();
  expect(tree).toMatchSnapshot();
});

it('displays update option when user belongs to buildings agency', () => {
  const { getByText } = render(
    <Router history={history}>
      <ParcelPopupView parcel={mockParcel('1')} />
    </Router>,
  );
  expect(getByText(/Update/i));
});

it('displays view option', () => {
  const { getByText } = render(
    <Router history={history}>
      <ParcelPopupView parcel={mockParcel('2')} />
    </Router>,
  );
  expect(getByText(/View/i));
});
