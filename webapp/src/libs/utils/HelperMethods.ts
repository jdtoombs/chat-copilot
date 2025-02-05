import { v4 } from 'uuid';

const getUUID = (): string => {
    return v4();
};

const maxBy = <T>(array: T[], iteratee: (i: T) => number | string) => {
    let result;
    let computed;
    for (const value of array) {
        const current = iteratee(value);

        if (computed === undefined ? current === current : current > computed) {
            computed = current;
            result = value;
        }
    }
    return result;
};

/**
 * Small helper function for attempting to convert a JSON formatted string to a JS string array.
 * Returns an empty array instead of throwing if anything fails.
 * @param str JSON string
 * @returns {string[]}
 */
const extractJsonArray = (str: string) => {
    try {
        const parsed = JSON.parse(str) as unknown;
        if (Array.isArray(parsed) && parsed.every((item) => typeof item === 'string')) {
            return parsed;
        } else {
            return [];
        }
    } catch (e) {
        return [];
    }
};

const isUrl = (str: string) => {
    try {
        new URL(str);
        return true;
    } catch {
        return false;
    }
};

export { extractJsonArray, getUUID, isUrl, maxBy };
