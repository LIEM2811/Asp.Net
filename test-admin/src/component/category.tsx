import { List, Datagrid, TextField, DeleteButton, EditButton, Create, Edit, SimpleForm, TextInput, DateField } from "react-admin";

export const CategoryList = () => (
    <List>
        <Datagrid>
            <TextField source="id" label="Category ID" />
            <TextField source="name" label="Category Name" />
            <TextField source="image" label="Image URL" />
            <DateField source="createdAt" label="Created At" />
            <DateField source="updatedAt" label="Updated At" />
            <EditButton />
            <DeleteButton />
        </Datagrid>
    </List>
);

export const CategoryCreate = () => (
    <Create>
        <SimpleForm>
            <TextInput source="name" label="Category Name" />
            <TextInput source="image" label="Image URL" />
        </SimpleForm>
    </Create>
);

export const CategoryEdit = () => (
    <Edit>
        <SimpleForm>
            <TextInput source="id" label="Category ID" disabled />
            <TextInput source="name" label="Category Name" />
            <TextInput source="image" label="Image URL" />
        </SimpleForm>
    </Edit>
);